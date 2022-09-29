using BPM.BLL.Extensions;
using BPM.BLL.Models;
using BPM.BLL.Utilities;
using BPM.Common.Utilities;
using BPM.DAL.DbContexts;
using BPM.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BPM.BLL.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AppDbContext _db;
        private readonly AuthorizationConfigs _authorizationConfigs;

        public AuthorizationService(
            AppDbContext db,
            AuthorizationConfigs authorizationConfigs)
        {
            _db = db;
            _authorizationConfigs = authorizationConfigs;
        }

        public async Task<AuthTokenModel> LoginAsync(AuthModel credentials)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Email.ToUpper() == credentials.Login.ToUpper() && x.StatusId == DAL.Enums.Status.Active);

            if (user == null || !SecurePasswordHasher.Verify(credentials.Password, user.Password))
            {
                throw new Exception("Not found");
            }

            return await GenerateTokenAsync(user);
        }

        private async Task<AuthTokenModel> GenerateTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authorizationConfigs.TokenKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(_authorizationConfigs.TokenExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshTokenModel = new RefreshToken()
            {
                UserId = user.Id,
                ExpiredIn = DateTime.UtcNow.AddMinutes(_authorizationConfigs.RefreshTokenExpirationInMinutes),
                Token = GenerateRefreshTokenAsync().Result
            };

            await _db.RefreshTokens.AddAsync(refreshTokenModel);
            await _db.SaveChangesAsync();

            return new AuthTokenModel
            {
                Token = tokenHandler.WriteToken(token),
                ExpiredIn = new DateTimeOffset(tokenDescriptor.Expires.Value).ToUnixTimeMilliseconds(),
                RefreshToken = refreshTokenModel.Token
            };
        }

        public async Task<AuthTokenModel> RefreshAsync(RefreshTokenRequestModel refreshTokenRequestModel)
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenRequestModel.Token);
            var userId = principal.GetCurrentUserId();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId && x.StatusId == DAL.Enums.Status.Active);
            var oldRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshTokenRequestModel.RefreshToken);

            if (user == null || oldRefreshToken == null)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            if (oldRefreshToken.IsExpired)
            {
                _db.RefreshTokens.Remove(oldRefreshToken);
                throw new SecurityTokenException("Refresh token is expired");
            }

            _db.RefreshTokens.Remove(oldRefreshToken);

            return await GenerateTokenAsync(user);
        }

        private async Task<string> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using (var rng  = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                var refreshToken = Convert.ToBase64String(randomNumber);
                var tokenIsUsed = await _db.RefreshTokens
                    .AnyAsync(x => x.Token.ToUpper() == refreshToken.ToUpper());

                if (tokenIsUsed)
                {
                    await GenerateRefreshTokenAsync();
                }

                return await Task.FromResult(refreshToken);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = TokenValidator.GetTokenValidationParameters(_authorizationConfigs.TokenKey);

            tokenValidationParameters.ValidateLifetime = false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}

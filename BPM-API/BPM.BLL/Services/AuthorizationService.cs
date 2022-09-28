using BPM.BLL.Models;
using BPM.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (user == null || )
            {

            }
        }

        public Task<AuthTokenModel> RefreshAsync(RefreshTokenRequestModel refreshTokenRequestModel)
        {
            throw new NotImplementedException();
        }
    }
}

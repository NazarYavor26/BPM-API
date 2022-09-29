using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BLL.Utilities
{
    public class TokenValidator
    {
        public static TokenValidationParameters GetTokenValidationParameters(string tokenKey)
        {
            var key = Encoding.ASCII.GetBytes(tokenKey);

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
        }
    }
}

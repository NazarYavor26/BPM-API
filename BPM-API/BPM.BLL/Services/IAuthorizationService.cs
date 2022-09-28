using BPM.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BLL.Services
{
    public interface IAuthorizationService
    {
        Task<AuthTokenModel> LoginAsync(AuthModel credentials);
        Task<AuthTokenModel> RefreshAsync(RefreshTokenRequestModel refreshTokenRequestModel);
    }
}

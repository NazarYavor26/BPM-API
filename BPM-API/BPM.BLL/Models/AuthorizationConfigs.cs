using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BLL.Models
{
    public class AuthorizationConfigs
    {
        public string TokenKey { get; set; }

         public int TokenExpirationInMinutes { get; set; }

        public int RefreshTokenExpirationInMinutes { get; set; }
    }
}

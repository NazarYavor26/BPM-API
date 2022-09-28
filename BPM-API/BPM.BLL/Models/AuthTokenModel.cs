using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BLL.Models
{
    public class AuthTokenModel
    {
        public string Token { get; set; }

        public long ExpiredIn { get; set; }

        public string RefreshToken { get; set; }
    }
}

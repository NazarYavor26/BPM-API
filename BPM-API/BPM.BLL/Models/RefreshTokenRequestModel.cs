using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BLL.Models
{
    public class RefreshTokenRequestModel
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}

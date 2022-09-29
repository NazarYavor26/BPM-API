using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BPM.BLL.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetCurrentUserId(this ClaimsPrincipal principal)
            => Int32.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier).Value, out var id)
            ? id
            : throw new Exception("NoPermission");
    }
}

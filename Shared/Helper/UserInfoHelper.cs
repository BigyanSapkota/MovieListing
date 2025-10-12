using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper
{
     public class UserInfoHelper
    {
        public static string? GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetUserEmail(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string? GetUserName(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string? GetUserRole(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string? GetUserPhone(ClaimsPrincipal user)
        {
            return user.FindFirst("PhoneNumber")?.Value;
        }

        public static Guid GetOrganizationId(ClaimsPrincipal user)
        {
            var orgIdClaim = user.FindFirst("OrganizationId")?.Value;

            if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var orgId))
            {
                throw new Exception("OrganizationId claim is missing or invalid");
            }

            return orgId;
        }

        public static string? GetOrganizationName(ClaimsPrincipal user)
        {
            return user.FindFirst("OrganizationName")?.Value;
        }



    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ThuocGiaThatAdmin.Server.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetCustomerId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(id))
                throw new UnauthorizedAccessException("CustomerId not found in token");

            return int.Parse(id);
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue("Id");

            if (string.IsNullOrEmpty(id))
                throw new UnauthorizedAccessException("UserId not found in token");

            return Guid.Parse(id);
        }
    }
}
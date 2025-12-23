using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MicroServiceApp.Shared.Services
{
    public class IdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
    {
        public Guid UserId
        {
            get
            {
                if (!httpContextAccessor.HttpContext!.User.Identity!.IsAuthenticated)
                    throw new UnauthorizedAccessException("User is not authenticated.");

                var user = httpContextAccessor.HttpContext?.User;
                if (user == null || !user.Identity!.IsAuthenticated)
                    throw new UnauthorizedAccessException("User is not authenticated.");

                var sub = user.FindFirst("sub")?.Value
                          ?? throw new UnauthorizedAccessException("sub claim missing");

                return Guid.Parse(sub);
            }
        }

        public string UserName
        {
            get
            {
                var user = httpContextAccessor.HttpContext!.User;
                // Literal isimleri kullanıyoruz
                return user.FindFirst("preferred_username")?.Value
                       ?? user.FindFirst("name")?.Value
                       ?? "Unknown";
            }
        }

        public List<string> Roles
        {
            get
            {
                // RoleClaimType'ı "roles" yaptığımız için .NET bunları Role tipinde içeri alır
                // Ama garanti olsun diye her iki ismi de kontrol edebilirsiniz
                return httpContextAccessor.HttpContext!.User.Claims
                    .Where(x => x.Type == "roles" || x.Type == "role")
                    .Select(x => x.Value!)
                    .ToList();
            }
        }
    }
}

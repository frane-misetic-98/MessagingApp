using System.Security.Claims;

namespace API.Extensions
{
    public static class HttpContextExtensions
    {
        public static int GetRequestUserId(this IHttpContextAccessor httpContextAccessor)
        {
            return int.Parse(httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}
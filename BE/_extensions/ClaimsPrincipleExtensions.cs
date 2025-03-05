using System.Security.Claims;

namespace BE._extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetName(this ClaimsPrincipal obj)
        {
            return obj.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
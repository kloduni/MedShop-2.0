using System.Security.Claims;

namespace MedShop.Extensions
{
    /// <summary>
    /// Extension methods on <see cref="ClaimsPrincipal"/> that provide convenient access to
    /// common Identity claim values without repeating the <see cref="ClaimTypes"/> constant lookups
    /// throughout controllers and views.
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        public static string Id(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}

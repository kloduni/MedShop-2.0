using MedShop.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedShop.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Toggle(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not found." });
            }

            try
            {
                var isAdded = await _wishlistService.ToggleWishlistAsync(productId, userId);

                return Json(new { success = true, isAdded = isAdded });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating your wishlist." });
            }
        }
    }
}
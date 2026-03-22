using MedShop.Core.Cart;
using MedShop.Core.Contracts;
using MedShop.Core.Models.ShoppingCart;
using MedShop.Extensions;
using Microsoft.AspNetCore.Mvc;
using static MedShop.Core.Constants.Cart.ShoppingCartConstants;
using static MedShop.Core.Constants.MessageConstants;
using static MedShop.Core.Constants.Product.ProductConstants;
using static MedShop.Core.Constants.User.AdminConstants;

namespace MedShop.Controllers
{
    public class ShoppingCartController : BaseController
    {
        private readonly ShoppingCart shoppingCart;
        private readonly IProductService productService;

        public ShoppingCartController(ShoppingCart _shoppingCart, IProductService _productService)
        {
            shoppingCart = _shoppingCart;
            productService = _productService;
        }

        public async Task<IActionResult> ShoppingCart()
        {

            var items = shoppingCart.GetShoppingCartItems();

            if (items.Count == 0)
            {
                TempData[WarningMessage] = CartIsEmpty;

                return RedirectToAction("All", "Product");
            }
            shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartViewModel()
            {
                ShoppingCart = shoppingCart,
                ShoppingCartTotal = await shoppingCart.GetShoppingCartTotalAsync()
            };

            return View(response);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddItemJson(int id)
        {
            var product = await productService.GetProductByIdAsync(id);

            if (product == null)
                return Json(new { success = false, message = ProductDoesNotExist });

            if (product.UsersProducts.Any(up => up.UserId == User.Id()) && User.IsInRole(AdminRoleName) == false)
                return Json(new { success = false, message = ProductBelongsToUser });

            var cartItems = shoppingCart.GetShoppingCartItems();
            var existingCartItem = cartItems.FirstOrDefault(i => i.Product.Id == id);
            int amountAlreadyInCart = existingCartItem?.Amount ?? 0;

            if (amountAlreadyInCart >= product.Quantity)
                return Json(new { success = false, message = ProductQuantityDepleted });

            await shoppingCart.AddItemToCartAsync(product);

            shoppingCart.ShoppingCartItems = null;
            var updatedItems = shoppingCart.GetShoppingCartItems();
            var updatedItem = updatedItems.FirstOrDefault(i => i.Product.Id == id);

            int newAmount = updatedItem?.Amount ?? 0;
            decimal subtotal = newAmount * product.Price;
            double grandTotal = await shoppingCart.GetShoppingCartTotalAsync();
            int totalItems = updatedItems.Sum(i => i.Amount);

            return Json(new
            {
                success = true,
                newAmount = newAmount,
                subtotalFormatted = subtotal.ToString("c"),
                grandTotalFormatted = grandTotal.ToString("c"),
                grandTotalRaw = grandTotal,
                cartCount = totalItems,
                message = "Added to cart!"
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> RemoveItemJson(int id)
        {
            var cartItem = await shoppingCart.GetCartItemByIdAsync(id);

            if (cartItem == null)
                return Json(new { success = false, message = "Item not found" });

            await shoppingCart.RemoveItemFromCartAsync(cartItem);

            shoppingCart.ShoppingCartItems = null;
            var updatedItems = shoppingCart.GetShoppingCartItems();
            var updatedItem = updatedItems.FirstOrDefault(i => i.Id == id);

            int newAmount = updatedItem?.Amount ?? 0;

            decimal price = cartItem.Product?.Price ?? 0;
            if (price == 0 && updatedItem != null) price = updatedItem.Product.Price;

            decimal subtotal = newAmount * price;
            double grandTotal = await shoppingCart.GetShoppingCartTotalAsync();
            int totalItems = updatedItems.Sum(i => i.Amount);

            return Json(new
            {
                success = true,
                newAmount = newAmount,
                subtotalFormatted = subtotal.ToString("c"),
                grandTotalFormatted = grandTotal.ToString("c"),
                grandTotalRaw = grandTotal,
                cartCount = totalItems
            });
        }

        public async Task<IActionResult> ClearCart()
        {
            await shoppingCart.ClearShoppingCartAsync();

            return RedirectToAction(nameof(ShoppingCart));
        }
    }
}

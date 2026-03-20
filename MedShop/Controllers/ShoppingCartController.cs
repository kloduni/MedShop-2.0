using MedShop.Core.Models.ShoppingCart;
using MedShop.Extensions;
using Microsoft.AspNetCore.Mvc;
using MedShop.Core.Contracts;
using MedShop.Core.Cart;
using static MedShop.Core.Constants.MessageConstants;
using static MedShop.Core.Constants.Cart.ShoppingCartConstants;
using static MedShop.Core.Constants.Product.ProductConstants;
using static MedShop.Areas.Admin.AdminConstants;

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


        public async Task<IActionResult> AddItemToShoppingCartAsync(int id)
        {
            var product = await productService.GetProductByIdAsync(id);

            if (product == null)
            {
                TempData[ErrorMessage] = ProductDoesNotExist;

                return RedirectToAction("All", "Product");
            }

            if (product.UsersProducts.Any(up => up.UserId == User.Id()) && User.IsInRole(AdminRoleName) == false)
            {
                TempData[WarningMessage] = ProductBelongsToUser;

                return RedirectToAction("All", "Product");
            }

            if (product.Quantity <= 0)
            {
                TempData[ErrorMessage] = ProductQuantityDepleted;

                return RedirectToAction("All", "Product");
            }


            await shoppingCart.AddItemToCartAsync(product);


            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveItemFromShoppingCartAsync(int id)
        {
            var cartItem = await shoppingCart.GetCartItemByIdAsync(id);

            if (cartItem != null)
            {
                await shoppingCart.RemoveItemFromCartAsync(cartItem);
            }

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> ClearCart()
        {
            await shoppingCart.ClearShoppingCartAsync();

            return RedirectToAction(nameof(ShoppingCart));
        }
    }
}

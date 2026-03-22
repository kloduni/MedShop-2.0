using MedShop.Core.Cart;
using MedShop.Core.Contracts;
using MedShop.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MedShop.Core.Constants.Cart.ShoppingCartConstants;
using static MedShop.Core.Constants.MessageConstants;

namespace MedShop.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly ShoppingCart shoppingCart;

        public OrderController(IOrderService _orderService, IProductService _productService, ShoppingCart _shoppingCart)
        {
            orderService = _orderService;
            productService = _productService;
            shoppingCart = _shoppingCart;
        }

        public async Task<IActionResult> All()
        {
            string userId = User.Id();
            var model = await orderService.GetOrdersByUserIdAsync(userId);

            return View(model);
        }

        public async Task<IActionResult> CompleteOrder()
        {
            string userId = User.Id();

            if (HttpContext.Session.GetString("UserId") != userId)
            {
                TempData[ErrorMessage] = WrongAccount;

                return RedirectToAction("Logout", "User");
            }

            var items = shoppingCart.GetShoppingCartItems();

            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            // Reduce stock quantities, persist the order, then clear the cart so it is
            // empty for the next purchase session.
            await productService.ReduceProductAmount(items);
            await orderService.StoreOrderAsync(items, userId, userEmailAddress);
            await shoppingCart.ClearShoppingCartAsync();

            return View("OrderCompleted");
        }
    }
}

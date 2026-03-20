using MedShop.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using MedShop.Extensions;
using MedShop.Core.Cart;
using System.Security.Claims;
using static MedShop.Core.Constants.MessageConstants;
using static MedShop.Core.Constants.Cart.ShoppingCartConstants;

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

            await productService.ReduceProductAmount(items);
            await orderService.StoreOrderAsync(items, userId, userEmailAddress);
            await shoppingCart.ClearShoppingCartAsync();

            return View("OrderCompleted");
        }
    }
}

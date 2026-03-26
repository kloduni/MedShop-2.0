using MedShop.Core.Cart;
using MedShop.Core.Contracts;
using MedShop.Extensions;
using MedShop.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

            var items = shoppingCart.GetShoppingCartItems();

            // Capture ALL items before clearing the cart for the review list
            var reviewItems = items.Select(i => (
                Id: i.Product.Id,
                Name: i.Product.ProductName,
                Slug: ModelExtensions.GetInformationFromId(i.Product.Id)
            )).ToList();

            ViewBag.ReviewItems = reviewItems;

            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            await productService.ReduceProductAmount(items);
            await orderService.StoreOrderAsync(items, userId, userEmailAddress);
            await shoppingCart.ClearShoppingCartAsync();

            return View("OrderCompleted");
        }
    }
}
using MedShop.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MedShop.Core.Constants.User.AdminConstants;

namespace MedShop.Areas.Admin.Controllers
{
    [Area(AreaName)]
    [Authorize(Roles = AdminRoleName)]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService _orderService)
        {
            orderService = _orderService;
        }

        public async Task<IActionResult> All()
        {
            var model = await orderService.GetAllOrdersAsync();

            return View(model);
        }
    }
}
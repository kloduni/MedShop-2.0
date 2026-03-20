using MedShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MedShop.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using static MedShop.Areas.Admin.AdminConstants;

namespace MedShop.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductService productService;
        private readonly ILogger logger;

        public HomeController(IProductService _productService, ILogger<HomeController> _logger)
        {
            productService = _productService;
            logger = _logger;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole(AdminRoleName))
            {
                return RedirectToAction("Index", "Home", new { area = AreaName });
            }

            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("All", "Product");
            }

            var model = await productService.AllCarousel();

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            logger.LogError(feature.Error, "TraceIdentifier: {0}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
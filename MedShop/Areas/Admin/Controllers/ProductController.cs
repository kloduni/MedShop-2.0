using MedShop.Core.Contracts;
using MedShop.Models;
using Microsoft.AspNetCore.Mvc;
using static MedShop.Core.Constants.MessageConstants;
using static MedShop.Core.Constants.Product.ProductConstants;
using static MedShop.Core.Constants.User.AdminConstants;

namespace MedShop.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService productService;

        public ProductController(IProductService _productService)
        {
            productService = _productService;
        }

        [HttpGet]
        public async Task<IActionResult> DeletedProducts([FromQuery] AllProductsQueryModel query)
        {
            if (!ModelState.IsValid)
            {
                // If the URL parameters are garbage, just return the default view
                return View(new AllProductsQueryModel());
            }

            var result = await productService.AllDeletedProducts(
                query.Category,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                query.ProductsPerPage);

            query.TotalProductsCount = result.TotalProductsCount;
            query.Categories = await productService.AllCategoriesNamesAsync();
            query.Products = result.Products;

            return View(query);
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            if ((await productService.ExistsAsync(id)) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotExist;
                return RedirectToAction(nameof(DeletedProducts));
            }

            TempData[SuccessMessage] = ProductRestored;
            await productService.RestoreProductAsync(id);

            return RedirectToAction(nameof(DeletedProducts));
        }
    }
}

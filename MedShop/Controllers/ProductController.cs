using MedShop.Core.Contracts;
using MedShop.Core.Extensions;
using MedShop.Core.Models.Product;
using MedShop.Extensions;
using MedShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MedShop.Core.Constants.Product.ProductConstants;
using static MedShop.Core.Constants.MessageConstants;
using static MedShop.Areas.Admin.AdminConstants;

namespace MedShop.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService productService;

        public ProductController(IProductService _productService)
        {
            productService = _productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> All([FromQuery]AllProductsQueryModel query)
        {
            var result = await productService.All(
                query.Category,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllProductsQueryModel.ProductsPerPage);

            query.TotalProductsCount = result.TotalProductsCount;
            query.Categories = await productService.AllCategoriesNamesAsync();
            query.Products = result.Products;

            return View(query);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {

            var model = new ProductBaseModel()
            {
                ProductCategories = await productService.AllCategoriesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductBaseModel model)
        {

            if (await productService.CategoryExistsAsync(model.CategoryId) == false)
            {
                ModelState.AddModelError(nameof(model.CategoryId), CategoryDoesNotExist);
            }

            if (!ModelState.IsValid)
            {
                model.ProductCategories = await productService.AllCategoriesAsync();
                return View(model);
            }

            var userId = User.Id();

            int id = await productService.CreateAsync(model, userId);

            TempData[SuccessMessage] = ProductAdded;

            return RedirectToAction(nameof(Details), new {id = id, information = model.GetInformation()});
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id, string information)
        {
            if (await productService.ExistsAsync(id) == false)
            {
                return RedirectToAction(nameof(All));
            }

            var model = await productService.ProductDetailsByIdAsync(id);

            if (information != model.GetInformation())
            {
                TempData[ErrorMessage] = NoExperiments;

                return RedirectToAction(nameof(All));
            }

            return View(model);
        }

        public async Task<IActionResult> MyProducts()
        {
            IEnumerable<ProductServiceModel> myProducts;
            var userId = User.Id();

            myProducts = await productService.AllProductsByUserIdAsync(userId);

            return View(myProducts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if ((await productService.ExistsAsync(id)) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotExist;

                return RedirectToAction(nameof(All));
            }

            if ((await productService.HasUserWithIdAsync(id, User.Id())) == false && User.IsInRole(AdminRoleName) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotBelongToUser;

                return RedirectToAction(nameof(All));
            }

            var product = await productService.ProductDetailsByIdAsync(id);
            var categoryId = await productService.GetProductCategoryIdAsync(id);

            var model = new ProductBaseModel()
            {
                Id = id,
                CategoryId = categoryId,
                ProductName = product.ProductName,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Quantity = product.Quantity,
                ProductCategories = await productService.AllCategoriesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductBaseModel model, string information)
        {
            if ((await productService.ExistsAsync(model.Id)) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotExist;
                model.ProductCategories = await productService.AllCategoriesAsync();

                return View(model);
            }

            if ((await productService.HasUserWithIdAsync(model.Id, User.Id())) == false && User.IsInRole(AdminRoleName) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotBelongToUser;

                return RedirectToAction(nameof(All));
            }

            if ((await productService.CategoryExistsAsync(model.CategoryId)) == false)
            {
                ModelState.AddModelError(nameof(model.CategoryId), CategoryDoesNotExist);
                model.ProductCategories = await productService.AllCategoriesAsync();

                return View(model);
            }

            if (information != model.GetInformation())
            {
                TempData[ErrorMessage] = NoExperiments;

                return RedirectToAction(nameof(All));
            }

            if (ModelState.IsValid == false)
            {
                model.ProductCategories = await productService.AllCategoriesAsync();

                return View(model);
            }

            await productService.EditAsync(model.Id, model);

            TempData[SuccessMessage] = SuccessMessage;

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if ((await productService.ExistsAsync(id)) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotExist;

                return RedirectToAction(nameof(All));
            }

            if ((await productService.HasUserWithIdAsync(id, User.Id())) == false && User.IsInRole(AdminRoleName) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotBelongToUser;

                return RedirectToAction(nameof(All));
            }

            var product = await productService.ProductDetailsByIdAsync(id);
            var model = new ProductServiceModel()
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Category = product.Category,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Quantity = product.Quantity,
                Seller = product.Seller
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductServiceModel model, string information)
        {
            if ((await productService.ExistsAsync(model.Id)) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotExist;
                return RedirectToAction(nameof(All));
            }

            if ((await productService.HasUserWithIdAsync(model.Id, User.Id())) == false && User.IsInRole(AdminRoleName) == false)
            {
                TempData[ErrorMessage] = ProductDoesNotBelongToUser;

                return RedirectToAction(nameof(All));
            }

            if (information != model.GetInformation())
            {
                TempData[ErrorMessage] = NoExperiments;
                return RedirectToAction(nameof(All));
            }

            await productService.DeleteAsync(model.Id);

            TempData[SuccessMessage] = ProductDeleted;

            return RedirectToAction(nameof(All));
        }
    }
}

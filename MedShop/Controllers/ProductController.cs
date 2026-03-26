using MedShop.Core.Contracts;
using MedShop.Core.Extensions;
using MedShop.Core.Models.Product;
using MedShop.Extensions;
using MedShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MedShop.Core.Constants.MessageConstants;
using static MedShop.Core.Constants.Product.ProductConstants;
using static MedShop.Core.Constants.User.AdminConstants;

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
        public async Task<IActionResult> All([FromQuery] AllProductsQueryModel query)
        {

            if (!ModelState.IsValid)
            {
                return View(new AllProductsQueryModel());
            }
            // Get the User ID only if the user is actually logged in
            string? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = User.Id();
            }

            // Pass the userId down to the service so it can check the wishlist
            var result = await productService.All(
                query.Category,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                query.ProductsPerPage,
                userId);

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
            var model = await productService.ProductDetailsByIdAsync(id);

            // If model is null, the product doesn't exist
            if (model == null)
            {
                return RedirectToAction(nameof(All));
            }

            // SEO Slug Check
            if (information != model.GetInformation())
            {
                TempData[ErrorMessage] = NoExperiments;
                return RedirectToAction(nameof(All));
            }

            // Check purchase status if the user is logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                string userId = User.Id(); // Using your .Id() extension
                model.HasPurchased = await productService.HasUserPurchasedProductAsync(id, userId);
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
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(All));
            }

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
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(All));
            }

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
            if (!ModelState.IsValid)
            {
                TempData[ErrorMessage] = InvalidProductData;
                return RedirectToAction(nameof(All));
            }

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

        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddReview(ReviewFormModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[ErrorMessage] = "Invalid review submission. Please check your inputs.";
                return RedirectToAction(nameof(Details), new { id = model.ProductId, information = model.Title }); // Fallback info string
            }

            try
            {
                var userId = User.Id();

                // Prevent the seller from reviewing their own product
                if (await productService.HasUserWithIdAsync(model.ProductId, userId))
                {
                    TempData[ErrorMessage] = "You cannot review your own product.";
                    return RedirectToAction(nameof(Details), new { id = model.ProductId, information = model.Title });
                }

                await productService.AddReviewAsync(model.ProductId, userId, model.Title, model.Description, model.Rating);

                TempData[SuccessMessage] = "Thank you! Your review has been posted.";
            }
            catch (Exception)
            {
                TempData[ErrorMessage] = "An unexpected error occurred while posting your review.";
            }

            // In order to redirect to Details cleanly, we need to fetch the product to get the valid 'information' string
            var product = await productService.ProductDetailsByIdAsync(model.ProductId);

            return RedirectToAction(nameof(Details), new { id = model.ProductId, information = product.GetInformation() });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ToggleVisibility(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if ((await productService.HasUserWithIdAsync(id, User.Id())) == false && User.IsInRole(AdminRoleName) == false)
            {
                return Unauthorized();
            }

            var isNowVisible = await productService.ToggleVisibilityAsync(id);

            return Json(new { success = true, isVisible = isNowVisible });
        }
    }
}

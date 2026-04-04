using static MedShop.Core.Constants.Product.ProductConstants;
using MedShop.Core.Contracts;
using MedShop.Core.Exceptions;
using MedShop.Core.Models.Product;
using MedShop.Core.Models.Product.ProductSortingEnum;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedShop.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository repo;
        private readonly ILogger logger;
        private readonly IGuard guard;

        public ProductService(IRepository _repo, ILogger<ProductService> _logger, IGuard _guard)
        {
            repo = _repo;
            logger = _logger;
            guard = _guard;
        }

        /// <summary>
        /// Returns a paginated, filtered, and sorted page of active products.
        /// Filtering by category and/or a full-text search term is applied before sorting,
        /// then results are sliced with skip/take to support pagination.
        /// </summary>
        public async Task<ProductQueryModel> All(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 9, string? currentUserId = null)
        {
            var result = new ProductQueryModel();

            var products = repo.AllReadonly<Product>()
                .Where(p => p.IsActive && p.IsVisible);

            // If user is logged in, filter out products they are selling
            if (!string.IsNullOrEmpty(currentUserId))
            {
                products = products.Where(p => !p.UsersProducts.Any(up => up.UserId == currentUserId));
            }

            if (string.IsNullOrEmpty(category) == false)
            {
                products = products.Where(p => p.Category.Name == category);
            }

            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                searchTerm = $"%{searchTerm.ToLower()}%";
                products = products
                    .Where(p => EF.Functions.Like(p.ProductName.ToLower(), searchTerm) ||
                                EF.Functions.Like(p.Description.ToLower(), searchTerm) ||
                                EF.Functions.Like(p.Category.Name.ToLower(), searchTerm));
            }

            products = sorting switch
            {
                ProductSorting.Price => products.OrderBy(p => p.Price),
                _ => products.OrderByDescending(p => p.Id)
            };

            //Fetch user's wishlist IDs for efficient checking
            var userWishlistProductIds = new List<int>();
            if (!string.IsNullOrEmpty(currentUserId))
            {
                userWishlistProductIds = await repo.AllReadonly<WishlistItem>()
                    .Where(w => w.UserId == currentUserId)
                    .Select(w => w.ProductId)
                    .ToListAsync();
            }

            result.Products = await products
                .Skip((currentPage - 1) * productsPerPage)
                .Take(productsPerPage)
                .Select(p => new ProductServiceModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Category = p.Category.Name,
                    Quantity = p.Quantity,
                    Seller = p.UsersProducts.Select(up => up.User.UserName).First(),
                    SellerId = p.UsersProducts.Select(up => up.UserId).First(),
                    IsInWishlist = userWishlistProductIds.Contains(p.Id),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => (double)r.Rating) : 0.0
                })
                .ToListAsync();

            result.TotalProductsCount = await products.CountAsync();

            return result;
        }

        public async Task<IEnumerable<ProductServiceModel>> AllCarousel()
        {
            return await repo.AllReadonly<Product>()
                .Where(p => p.IsActive)
                .Select(p => new ProductServiceModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> AllCategoriesNamesAsync()
        {
            return await repo.AllReadonly<Category>()
                .Select(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductCategoryModel>> AllCategoriesAsync()
        {
            return await repo.AllReadonly<Category>()
                .OrderBy(c => c.Name)
                .Select(c => new ProductCategoryModel()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await repo.AllReadonly<Category>()
                .AnyAsync(c => c.Id == categoryId);

        }

        public async Task<int> CreateAsync(ProductBaseModel model, string userId)
        {
            var product = new Product()
            {
                ProductName = model.ProductName,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                Price = model.Price,
                Quantity = model.Quantity,
                CategoryId = model.CategoryId
            };

            var userProduct = new UserProduct()
            {
                UserId = userId,
                Product = product
            };

            try
            {
                await repo.AddAsync(product);
                await repo.AddAsync(userProduct);
                await repo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(nameof(CreateAsync), e);
                throw new ApplicationException(DbSaveError, e);
            }

            return product.Id;
        }

        public async Task<bool> ExistsAsync(int productId)
        {
            return await repo.AllReadonly<Product>()
                .AnyAsync(p => p.Id == productId);
        }

        public async Task<ProductServiceModel> ProductDetailsByIdAsync(int productId)
        {
            return await repo.AllReadonly<Product>()
                .Where(p => p.IsActive && p.Id == productId)
                .Select(p => new ProductServiceModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Category = p.Category.Name,
                    Quantity = p.Quantity,
                    Seller = p.UsersProducts.Select(up => up.User.UserName).First(),
                    SellerId = p.UsersProducts.Select(up => up.UserId).First(),

                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => (double)r.Rating) : 0.0,

                    Reviews = p.Reviews
                        .OrderByDescending(r => r.CreatedOn)
                        .Select(r => new ReviewServiceModel()
                        {
                            Id = r.Id,
                            Title = r.Title,
                            Description = r.Description,
                            Rating = r.Rating,
                            ReviewerName = r.User.UserName,
                            CreatedOn = r.CreatedOn.ToString("dd/MM/yyyy")
                        }).ToList()
                })
                .FirstAsync();
        }

        public async Task<IEnumerable<ProductServiceModel>> AllProductsByUserIdAsync(string userId)
        {
            return await repo.AllReadonly<Product>()
                .Where(p => p.IsActive && p.UsersProducts.Select(up => up.UserId).First() == userId)
                .Select(p => new ProductServiceModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Category = p.Category.Name,
                    Quantity = p.Quantity,
                    Seller = p.UsersProducts.Select(up => up.User.UserName).First(),
                    SellerId = p.UsersProducts.Select(up => up.UserId).First(),
                    IsVisible = p.IsVisible
                })
                .ToListAsync();
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="currentUserId"/> is the seller of the given
        /// product.  Used to prevent sellers from purchasing their own listings and to gate
        /// edit/delete access to the product owner.
        /// </summary>
        public async Task<bool> HasUserWithIdAsync(int productId, string currentUserId)
        {
            bool result = false;

            var product = await repo.AllReadonly<Product>()
                .Where(p => p.IsActive && p.Id == productId)
                .Include(p => p.UsersProducts)
                .FirstOrDefaultAsync();

            if (product != null && product.UsersProducts.Select(up => up.UserId).First() == currentUserId)
            {
                result = true;
            }

            return result;
        }

        public async Task<int> GetProductCategoryIdAsync(int productId)
        {
            return (await repo.GetByIdAsync<Product>(productId)).CategoryId;
        }

        public async Task EditAsync(int productId, ProductBaseModel model)
        {
            var product = await repo.GetByIdAsync<Product>(productId);
            guard.AgainstNull(product, ProductNotFound);

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.Quantity = model.Quantity;

            await repo.SaveChangesAsync();
        }

        /// <summary>
        /// Soft-deletes the product by setting <c>IsActive = false</c> rather than removing the
        /// row, preserving historical order data that references this product.
        /// </summary>
        public async Task DeleteAsync(int productId)
        {
            var product = await repo.GetByIdAsync<Product>(productId);
            guard.AgainstNull(product, ProductNotFound);
            product.IsActive = false;

            await repo.SaveChangesAsync();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await repo.All<Product>()
                .Include(p => p.UsersProducts)
                .FirstAsync(p => p.Id == productId);
        }

        /// <summary>
        /// Same filtering/sorting/pagination pipeline as <see cref="All"/>, but operates on
        /// soft-deleted (inactive) products for the admin "recycle bin" view.
        /// </summary>
        public async Task<ProductQueryModel> AllDeletedProducts(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 9)
        {

            var result = new ProductQueryModel();

            var products = repo.AllReadonly<Product>()
                .Include(p => p.Category)
                .Where(p => p.IsActive == false);

            if (string.IsNullOrEmpty(category) == false)
            {
                products = products.Where(p => p.Category.Name == category);
            }

            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                searchTerm = $"%{searchTerm.ToLower()}%";
                products = products
                    .Where(p => EF.Functions.Like(p.ProductName.ToLower(), searchTerm) ||
                                EF.Functions.Like(p.Description.ToLower(), searchTerm) ||
                                EF.Functions.Like(p.Category.Name.ToLower(), searchTerm));
            }

            products = sorting switch
            {
                ProductSorting.Price => products.OrderBy(p => p.Price),
                _ => products.OrderByDescending(p => p.Id)
            };

            result.Products = await products
                .Skip((currentPage - 1) * productsPerPage)
                .Take(productsPerPage)
                .Select(p => new ProductServiceModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Category = p.Category.Name,
                    Quantity = p.Quantity,
                    Seller = p.UsersProducts.Select(up => up.User.UserName).First(),
                    SellerId = p.UsersProducts.Select(up => up.UserId).First()
                })
                .ToListAsync();

            result.TotalProductsCount = await products.CountAsync();

            return result;

        }

        public async Task RestoreProductAsync(int id)
        {
            var product = await repo.GetByIdAsync<Product>(id);
            guard.AgainstNull(product, ProductNotFound);
            product.IsActive = true;

            await repo.SaveChangesAsync();
        }

        public async Task ReduceProductAmount(ICollection<ShoppingCartItem> items)
        {
            foreach (var item in items)
            {
                var product = await repo.All<Product>()
                    .FirstAsync(p => p.Id == item.Product.Id);
                guard.AgainstNull(product, ProductNotFound);
                product.Quantity -= item.Amount;

            }
            await repo.SaveChangesAsync();
        }

        public async Task<bool> ToggleVisibilityAsync(int productId)
        {
            var product = await repo.GetByIdAsync<Product>(productId);
            if (product == null) return false;

            product.IsVisible = !product.IsVisible;
            await repo.SaveChangesAsync();

            return product.IsVisible;
        }

        public async Task AddReviewAsync(int productId, string userId, string title, string description, int rating)
        {
            // Security Check: Make sure the product actually exists
            var productExists = await ExistsAsync(productId);
            guard.AgainstNull(productExists ? new object() : null, ProductNotFound);

            var review = new Review()
            {
                ProductId = productId,
                UserId = userId,
                Title = title,
                Description = description,
                Rating = rating,
                CreatedOn = DateTime.UtcNow
            };

            await repo.AddAsync(review);
            await repo.SaveChangesAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(int productId, string userId)
        {
            // Check the Orders table for any order belonging to the user that contains an OrderItem matching the ProductId
            return await repo.AllReadonly<Order>()
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId));
        }
    }
}

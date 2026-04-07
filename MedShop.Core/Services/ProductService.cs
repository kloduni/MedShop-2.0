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
        private readonly IApplicationDbContext context;
        private readonly ILogger logger;
        private readonly IGuard guard;

        public ProductService(IApplicationDbContext _context, ILogger<ProductService> _logger, IGuard _guard)
        {
            context = _context;
            logger = _logger;
            guard = _guard;
        }

        public async Task<ProductQueryModel> All(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 9, string? currentUserId = null)
        {
            var result = new ProductQueryModel();

            var products = context.Products.AsNoTracking()
                .Where(p => p.IsActive && p.IsVisible);

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
                string term = searchTerm.ToLower();
                products = products
                    .Where(p => p.ProductName.ToLower().Contains(term) ||
                                p.Description.ToLower().Contains(term) ||
                                p.Category.Name.ToLower().Contains(term));
            }

            products = sorting switch
            {
                ProductSorting.Price => products.OrderBy(p => p.Price),
                _ => products.OrderByDescending(p => p.Id)
            };

            var userWishlistProductIds = new List<int>();
            if (!string.IsNullOrEmpty(currentUserId))
            {
                userWishlistProductIds = await context.WishlistItems.AsNoTracking()
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

        public async Task<ProductQueryModel> AllHiddenProducts(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 9)
        {
            var result = new ProductQueryModel();

            var products = context.Products.AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.IsVisible == false);

            if (string.IsNullOrEmpty(category) == false)
            {
                products = products.Where(p => p.Category.Name == category);
            }

            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                string term = searchTerm.ToLower();
                products = products
                    .Where(p => p.ProductName.ToLower().Contains(term) ||
                                p.Description.ToLower().Contains(term) ||
                                p.Category.Name.ToLower().Contains(term));
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

        public async Task<IEnumerable<string>> AllCategoriesNamesAsync()
        {
            return await context.Categories.AsNoTracking()
                .Select(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductCategoryModel>> AllCategoriesAsync()
        {
            return await context.Categories.AsNoTracking()
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
            return await context.Categories.AsNoTracking()
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
                await context.Products.AddAsync(product);
                await context.UsersProducts.AddAsync(userProduct);
                await context.SaveChangesAsync();
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
            return await context.Products.AsNoTracking()
                .AnyAsync(p => p.Id == productId);
        }

        public async Task<ProductServiceModel> ProductDetailsByIdAsync(int productId)
        {
            return await context.Products.AsNoTracking()
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
            return await context.Products.AsNoTracking()
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

        public async Task<bool> HasUserWithIdAsync(int productId, string currentUserId)
        {
            bool result = false;

            var product = await context.Products.AsNoTracking()
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
            return (await context.Products.FindAsync(productId)).CategoryId;
        }

        public async Task EditAsync(int productId, ProductBaseModel model)
        {
            var product = await context.Products.FindAsync(productId);
            guard.AgainstNull(product, ProductNotFound);

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.Quantity = model.Quantity;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int productId)
        {
            var product = await context.Products.FindAsync(productId);
            guard.AgainstNull(product, ProductNotFound);
            product.IsActive = false;

            await context.SaveChangesAsync();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await context.Products
                .Include(p => p.UsersProducts)
                .FirstAsync(p => p.Id == productId);
        }

        public async Task<ProductQueryModel> AllDeletedProducts(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 9)
        {
            var result = new ProductQueryModel();

            var products = context.Products.AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsActive == false);

            if (string.IsNullOrEmpty(category) == false)
            {
                products = products.Where(p => p.Category.Name == category);
            }

            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                string term = searchTerm.ToLower();
                products = products
                    .Where(p => p.ProductName.ToLower().Contains(term) ||
                                p.Description.ToLower().Contains(term) ||
                                p.Category.Name.ToLower().Contains(term));
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
            var product = await context.Products.FindAsync(id);
            guard.AgainstNull(product, ProductNotFound);
            product.IsActive = true;

            await context.SaveChangesAsync();
        }

        public async Task ReduceProductAmount(ICollection<ShoppingCartItem> items)
        {
            foreach (var item in items)
            {
                var product = await context.Products
                    .FirstAsync(p => p.Id == item.Product.Id);
                guard.AgainstNull(product, ProductNotFound);
                product.Quantity -= item.Amount;
            }
            await context.SaveChangesAsync();
        }

        public async Task<bool> ToggleVisibilityAsync(int productId)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null) return false;

            product.IsVisible = !product.IsVisible;
            await context.SaveChangesAsync();

            return product.IsVisible;
        }

        public async Task AddReviewAsync(int productId, string userId, string title, string description, int rating)
        {
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

            await context.Reviews.AddAsync(review);
            await context.SaveChangesAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(int productId, string userId)
        {
            return await context.Orders.AsNoTracking()
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId));
        }
    }
}
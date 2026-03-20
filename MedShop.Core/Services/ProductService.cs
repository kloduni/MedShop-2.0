using MedShop.Core.Contracts;
using MedShop.Core.Exceptions;
using MedShop.Core.Models.Product;
using MedShop.Core.Models.Product.ProductSortingEnum;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
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
        /// Gets all products according to filters
        /// </summary>
        /// <param name="category"></param>
        /// <param name="searchTerm"></param>
        /// <param name="sorting"></param>
        /// <param name="currentPage"></param>
        /// <param name="productsPerPage"></param>
        /// <returns></returns>
        public async Task<ProductQueryModel> All(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 1)
        {
            var result = new ProductQueryModel();

            var products = repo.AllReadonly<Product>()
                .Where(p => p.IsActive);

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
                    Seller = p.UsersProducts.Select(up => up.User.UserName).First()
                })
                .ToListAsync();

            result.TotalProductsCount = await products.CountAsync();

            return result;
        }

        /// <summary>
        /// Gets all products for carousel
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all category names
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> AllCategoriesNamesAsync()
        {
            return await repo.AllReadonly<Category>()
                .Select(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if category exists by category Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await repo.AllReadonly<Category>()
                .AnyAsync(c => c.Id == categoryId);

        }

        /// <summary>
        /// Creates new product
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
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
                throw new ApplicationException("Failed to save in Db", e);
            }

            return product.Id;
        }

        /// <summary>
        /// Checks if product exists
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(int productId)
        {
            return await repo.AllReadonly<Product>()
                .AnyAsync(p => p.Id == productId);
        }

        /// <summary>
        /// Gets product details by product Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
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
                    Seller = p.UsersProducts.Select(up => up.User.UserName).First()
                })
                .FirstAsync();
        }

        /// <summary>
        /// Gets all user products by user Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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
                    Seller = p.UsersProducts.Select(up => up.User.UserName).First()
                })
                .ToListAsync();
        }

        /// <summary>
        /// Checks if product has user by user Id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets product category Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<int> GetProductCategoryIdAsync(int productId)
        {
            return (await repo.GetByIdAsync<Product>(productId)).CategoryId;
        }

        public async Task EditAsync(int productId, ProductBaseModel model)
        {
            var product = await repo.GetByIdAsync<Product>(productId);
            guard.AgainstNull(product, "Product not found!");

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.Quantity = model.Quantity;

            await repo.SaveChangesAsync();
        }

        /// <summary>
        /// Marks product as inactive in Db
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int productId)
        {
            var product = await repo.GetByIdAsync<Product>(productId);
            guard.AgainstNull(product, "Product not found!");
            product.IsActive = false;

            await repo.SaveChangesAsync();
        }

        /// <summary>
        /// Gets product by product Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await repo.All<Product>()
                .Include(p => p.UsersProducts)
                .FirstAsync(p => p.Id == productId);
        }

        /// <summary>
        /// Gets all deleted products by given filters
        /// </summary>
        /// <param name="category"></param>
        /// <param name="searchTerm"></param>
        /// <param name="sorting"></param>
        /// <param name="currentPage"></param>
        /// <param name="productsPerPage"></param>
        /// <returns></returns>
        public async Task<ProductQueryModel> AllDeletedProducts(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 1)
        {
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
                        Seller = p.UsersProducts.Select(up => up.User.UserName).First()
                    })
                    .ToListAsync();

                result.TotalProductsCount = await products.CountAsync();

                return result;
            }
        }

        /// <summary>
        /// Restores product active status in Db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RestoreProductAsync(int id)
        {
            var product = await repo.GetByIdAsync<Product>(id);
            guard.AgainstNull(product, "Product not found!");
            product.IsActive = true;

            await repo.SaveChangesAsync();
        }

        /// <summary>
        /// Reduces product amount by amount in cart items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task ReduceProductAmount(ICollection<ShoppingCartItem> items)
        {
            foreach (var item in items)
            {
                var product = await repo.All<Product>()
                    .FirstAsync(p => p.Id == item.Product.Id);
                guard.AgainstNull(product, "Product not found!");
                product.Quantity -= item.Amount;

            }
            await repo.SaveChangesAsync();
        }
    }
}

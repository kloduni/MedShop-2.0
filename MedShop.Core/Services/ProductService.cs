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
        /// Returns a paginated, filtered, and sorted page of active products.
        /// Filtering by category and/or a full-text search term is applied before sorting,
        /// then results are sliced with skip/take to support pagination.
        /// </summary>
        public async Task<ProductQueryModel> All(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 8)
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
                    Seller = p.UsersProducts.Select(up => up.User.UserName).First(),
                    SellerId = p.UsersProducts.Select(up => up.UserId).First()
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
                throw new ApplicationException("Failed to save in Db", e);
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
                    SellerId = p.UsersProducts.Select(up => up.UserId).First()
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
                    SellerId = p.UsersProducts.Select(up => up.UserId).First()
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
        /// Soft-deletes the product by setting <c>IsActive = false</c> rather than removing the
        /// row, preserving historical order data that references this product.
        /// </summary>
        public async Task DeleteAsync(int productId)
        {
            var product = await repo.GetByIdAsync<Product>(productId);
            guard.AgainstNull(product, "Product not found!");
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
        public async Task<ProductQueryModel> AllDeletedProducts(string? category = null, string? searchTerm = null, ProductSorting sorting = ProductSorting.Newest, int currentPage = 1, int productsPerPage = 8)
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
            guard.AgainstNull(product, "Product not found!");
            product.IsActive = true;

            await repo.SaveChangesAsync();
        }

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

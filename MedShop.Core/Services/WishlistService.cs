using MedShop.Core.Contracts;
using MedShop.Core.Models.Product;
using MedShop.Infrastructure.Data.Common;
using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedShop.Core.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IRepository repo;
        private readonly ILogger<WishlistService> logger;

        public WishlistService(IRepository _repo, ILogger<WishlistService> _logger)
        {
            repo = _repo;
            logger = _logger;
        }

        public async Task<bool> ToggleWishlistAsync(int productId, string userId)
        {
            var existingItem = await repo.All<WishlistItem>()
                .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);

            if (existingItem != null)
            {
                repo.Delete(existingItem);
                await repo.SaveChangesAsync();
                return false;
            }
            else
            {
                var newItem = new WishlistItem
                {
                    ProductId = productId,
                    UserId = userId
                };

                await repo.AddAsync(newItem);
                await repo.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> IsInWishlistAsync(int productId, string userId)
        {
            return await repo.AllReadonly<WishlistItem>()
                .AnyAsync(w => w.ProductId == productId && w.UserId == userId);
        }

        public async Task<IEnumerable<ProductServiceModel>> GetUserWishlistAsync(string userId)
        {
            return await repo.AllReadonly<WishlistItem>()
                .Where(w => w.UserId == userId && w.Product.IsActive)
                .Select(w => new ProductServiceModel
                {
                    Id = w.Product.Id,
                    ProductName = w.Product.ProductName,
                    Description = w.Product.Description,
                    ImageUrl = w.Product.ImageUrl,
                    Price = w.Product.Price,
                    Category = w.Product.Category.Name,
                    Quantity = w.Product.Quantity,
                    Seller = w.Product.UsersProducts.Select(up => up.User.UserName).First(),
                    SellerId = w.Product.UsersProducts.Select(up => up.UserId).First(),
                    IsVisible = w.Product.IsVisible,
                    IsInWishlist = true
                })
                .ToListAsync();
        }
    }
}
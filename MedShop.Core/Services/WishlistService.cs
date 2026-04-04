using MedShop.Core.Contracts;
using MedShop.Core.Models.Product;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedShop.Core.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IApplicationDbContext context;
        private readonly ILogger<WishlistService> logger;

        public WishlistService(IApplicationDbContext _context, ILogger<WishlistService> _logger)
        {
            context = _context;
            logger = _logger;
        }

        public async Task<bool> ToggleWishlistAsync(int productId, string userId)
        {
            var existingItem = await context.WishlistItems
                .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);

            if (existingItem != null)
            {
                context.WishlistItems.Remove(existingItem);
                await context.SaveChangesAsync();
                return false;
            }
            else
            {
                var newItem = new WishlistItem
                {
                    ProductId = productId,
                    UserId = userId
                };

                await context.WishlistItems.AddAsync(newItem);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> IsInWishlistAsync(int productId, string userId)
        {
            return await context.WishlistItems.AsNoTracking()
                .AnyAsync(w => w.ProductId == productId && w.UserId == userId);
        }

        public async Task<IEnumerable<ProductServiceModel>> GetUserWishlistAsync(string userId)
        {
            return await context.WishlistItems.AsNoTracking()
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
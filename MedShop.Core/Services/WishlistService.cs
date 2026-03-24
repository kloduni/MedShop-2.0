using MedShop.Core.Contracts;
using MedShop.Infrastructure.Data;
using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Core.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;

        public WishlistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleWishlistAsync(int productId, string userId)
        {
            // Look for an existing wishlist record
            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);

            if (existingItem != null)
            {
                // It exists, so remove it (User is "un-hearting" it)
                _context.WishlistItems.Remove(existingItem);
                await _context.SaveChangesAsync();
                return false;
            }
            else
            {
                // It doesn't exist, so add it (User is "hearting" it)
                var newItem = new WishlistItem
                {
                    ProductId = productId,
                    UserId = userId
                };
                _context.WishlistItems.Add(newItem);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> IsInWishlistAsync(int productId, string userId)
        {
            return await _context.WishlistItems
                .AnyAsync(w => w.ProductId == productId && w.UserId == userId);
        }
    }
}
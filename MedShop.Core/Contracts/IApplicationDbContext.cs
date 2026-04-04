using Microsoft.AspNetCore.Identity;
using MedShop.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Core.Contracts
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<User> Users { get; }
        DbSet<IdentityRole> Roles { get; }
        DbSet<IdentityUserRole<string>> UserRoles { get; }
        DbSet<Category> Categories { get; }
        DbSet<Review> Reviews { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<ShoppingCartItem> ShoppingCartItems { get; }
        DbSet<WishlistItem> WishlistItems { get; }
        DbSet<UserProduct> UsersProducts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
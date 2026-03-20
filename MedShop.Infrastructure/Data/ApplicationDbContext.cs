using MedShop.Infrastructure.Data.Configuration;
using MedShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private bool seedDb;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, bool seed = true)
            : base(options)
        {
            // UNCOMMENT WHEN CREATING DB

            seedDb = seed;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserProduct> UsersProducts { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserProduct>()
                .HasKey(up => new {up.UserId, up.ProductId});

            if (seedDb)
            {
                builder.ApplyConfiguration(new UserConfiguration());
                builder.ApplyConfiguration(new CategoryConfiguration());
                builder.ApplyConfiguration(new ProductConfiguration());
                builder.ApplyConfiguration(new UserProductConfiguration());
            }

            base.OnModelCreating(builder);
        }
    }
}
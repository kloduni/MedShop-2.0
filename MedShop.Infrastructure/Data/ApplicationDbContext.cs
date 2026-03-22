using MedShop.Infrastructure.Data.Configuration;
using MedShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedShop.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private bool seedDb;

        /// <param name="seed">
        /// Pass <c>false</c> in unit tests to skip seed data so migrations aren't applied against
        /// an in-memory database and foreign-key constraints remain predictable.
        /// </param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, bool seed = true)
            : base(options)
        {
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
            // UserProduct uses a composite primary key to model the many-to-many relationship
            // between users (sellers) and products.
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
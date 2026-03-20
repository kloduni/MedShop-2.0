using MedShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedShop.Infrastructure.Data.Configuration
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);
            
           builder.HasData(CreateUsersList());
        }

        private List<User> CreateUsersList()
        {
            var users = new List<User>();
            var hasher = new PasswordHasher<User>();

            var user = new User()
            {
                Id = "dea12856-c198-4129-b3f3-b893d8395082",
                UserName = "admin",
                NormalizedUserName = "admin",
                Email = "admin@medshop.com",
                NormalizedEmail = "admin@medshop.com",
            };
            user.PasswordHash = hasher.HashPassword(user, "admin");
            users.Add(user);

            user = new User()
            {
                Id = "89159c08-2f95-456f-91ea-75136c030b7b",
                UserName = "guest",
                NormalizedUserName = "guest",
                Email = "guest@medshop.com",
                NormalizedEmail = "guest@medshop.com"
            };
            user.PasswordHash = hasher.HashPassword(user, "guest");
            users.Add(user);

            user = new User()
            {
                Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                UserName = "guest1",
                NormalizedUserName = "guest1",
                Email = "guest1@medshop.com",
                NormalizedEmail = "guest1@medshop.com"
            };
            user.PasswordHash = hasher.HashPassword(user, "guest");
            users.Add(user);


            return users;
        }
    }
}

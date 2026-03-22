using MedShop.Infrastructure.Data.Models;
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

            var admin = new User()
            {
                Id = "dea12856-c198-4129-b3f3-b893d8395082",
                UserName = "admin",
                NormalizedUserName = "admin",
                Email = "admin@medshop.com",
                NormalizedEmail = "admin@medshop.com",
                PasswordHash = "AQAAAAIAAYagAAAAEEC93LkGA1IblokFJD/R69GtPP5iUOvhMRI0EeT257PPE5qr8DZdn4TnoBZcG+YPDA==",
                // ConcurrencyStamp and SecurityStamp are hardcoded so that every database
                // creation produces identical seed rows and migration snapshots stay stable.
                ConcurrencyStamp = "8c350119-9236-407d-a169-2fdf07e4d283",
                SecurityStamp = "0a5b8207-6bb3-4d2c-8ab5-f2d4e7de4eb0"
            };
            users.Add(admin);

            var guest = new User()
            {
                Id = "89159c08-2f95-456f-91ea-75136c030b7b",
                UserName = "guest",
                NormalizedUserName = "guest",
                Email = "guest@medshop.com",
                NormalizedEmail = "guest@medshop.com",
                PasswordHash = "AQAAAAIAAYagAAAAEJKa30JvACzHXUvKYL463Ov4nPvw2uKouDeMxPQPe9V0JQmWnghIg7tkLioViIrpHQ==",
                ConcurrencyStamp = "e95222c5-7f4c-47fc-8f6a-fbaeb0ccdaaf",
                SecurityStamp = "7b686259-873b-486d-b8de-fae7826359eb"
            };
            users.Add(guest);

            var guest1 = new User()
            {
                Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                UserName = "guest1",
                NormalizedUserName = "guest1",
                Email = "guest1@medshop.com",
                NormalizedEmail = "guest1@medshop.com",
                PasswordHash = "AQAAAAIAAYagAAAAEJB9OkGYgADBwQDJcDPt/IUTtyPnO/fDaeHjGK9rVbuc7cyEMWLN50zXsRExiITBHw==",
                ConcurrencyStamp = "f2868ff8-e6b7-4a00-bf69-7ee4a66a1e8a",
                SecurityStamp = "1d5ef264-b52a-4fdf-9767-f584fdf6e64c"
            };
            users.Add(guest1);

            return users;
        }
    }
}
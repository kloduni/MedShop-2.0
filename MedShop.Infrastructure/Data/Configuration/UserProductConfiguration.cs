using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedShop.Infrastructure.Data.Configuration
{
    internal class UserProductConfiguration : IEntityTypeConfiguration<UserProduct>
    {
        public void Configure(EntityTypeBuilder<UserProduct> builder)
        {
            builder.HasData(CreateUsersProductsList());
        }

        private List<UserProduct> CreateUsersProductsList()
        {
            var usersProducts = new List<UserProduct>()
            {
                new UserProduct()
                {
                    UserId = "dea12856-c198-4129-b3f3-b893d8395082",
                    ProductId = 1
                },

                new UserProduct()
                {
                    UserId = "dea12856-c198-4129-b3f3-b893d8395082",
                    ProductId = 2
                },

                new UserProduct()
                {
                    UserId = "dea12856-c198-4129-b3f3-b893d8395082",
                    ProductId = 3
                },

                new UserProduct()
                {
                    UserId = "dea12856-c198-4129-b3f3-b893d8395082",
                    ProductId = 4
                },                
                
                new UserProduct()
                {
                    UserId = "89159c08-2f95-456f-91ea-75136c030b7b",
                    ProductId = 5
                },

                new UserProduct()
                {
                    UserId = "89159c08-2f95-456f-91ea-75136c030b7b",
                    ProductId = 6
                },

                new UserProduct()
                {
                    UserId = "89159c08-2f95-456f-91ea-75136c030b7b",
                    ProductId = 7
                },

                new UserProduct()
                {
                    UserId = "89159c08-2f95-456f-91ea-75136c030b7b",
                    ProductId = 8
                },

                new UserProduct()
                {
                    UserId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                    ProductId = 9
                },

                new UserProduct()
                {
                    UserId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                    ProductId = 10
                },

                new UserProduct()
                {
                    UserId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                    ProductId = 11
                },

                new UserProduct()
                {
                    UserId = "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                    ProductId = 12
                },
            };

            return usersProducts;
        }
    }
}

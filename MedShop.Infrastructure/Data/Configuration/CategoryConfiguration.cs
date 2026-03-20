using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedShop.Infrastructure.Data.Configuration
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(CreateCategories());
        }

        private List<Category> CreateCategories()
        {
            var categories = new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    Name = "Urology"
                },

                new Category()
                {
                    Id = 2,
                    Name = "Cardiology"
                },

                new Category()
                {
                    Id = 3,
                    Name = "Orthopedy"
                },

                new Category()
                {
                    Id = 4,
                    Name = "Surgery"
                },

                new Category()
                {
                    Id = 5,
                    Name = "ENT"
                },

                new Category()
                {
                    Id = 6,
                    Name = "Skin"
                },

                new Category()
                {
                    Id = 7,
                    Name = "General"
                }
            };

            return categories;
        }
    }
}

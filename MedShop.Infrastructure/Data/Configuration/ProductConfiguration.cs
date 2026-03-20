using MedShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedShop.Infrastructure.Data.Configuration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(CreateProducts());
        }

        private List<Product> CreateProducts()
        {
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    ProductName = "Catheter",
                    Description = "Used for urination complications.",
                    ImageUrl = "https://www.bbraun.com/content/dam/catalog/bbraun/bbraunProductCatalog/S/AEM2015/en-01/b8/vasofix-braunuele.jpeg.transform/75/image.jpg",
                    Price = 13.76m,
                    CategoryId = 1,
                    Quantity = 10
                },
                new Product
                {
                    Id = 2,
                    ProductName = "Spatula",
                    Description = "General instrument.",
                    ImageUrl = "https://www.bbraun-vetcare.com/content/dam/b-braun/global/website/veterinary/products-and-therapies/wound-therapy-and-wound-closure/text_image_nadeln_DLM.jpg.transform/600/image.jpg",
                    Price = 1.50m,
                    CategoryId = 7,
                    Quantity = 0
                },

                new Product
                {
                    Id = 3,
                    ProductName = "Scalpel",
                    Description = "Used for scalping.",
                    ImageUrl = "https://www.carlroth.com/medias/3607-1000Wx1000H?context=bWFzdGVyfGltYWdlc3w1NjMxNnxpbWFnZS9qcGVnfGltYWdlcy9oOTYvaGM5Lzg4MjIxNDM5NzU0NTQuanBnfGMzZDZlODk0YmE0Y2MyZWE2MmU2ZTA2ZjkxNTNjOGI3MWMyMjgyYzZmNmFjOWFjOTAwMzY5ZjJjNDVkOGEyNTE",
                    Price = 2.50m,
                    CategoryId = 3,
                    Quantity = 10
                },

                new Product
                {
                    Id = 4,
                    ProductName = "Forceps",
                    Description = "Used for cutting.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/55/Forceps_plastic.jpg/1200px-Forceps_plastic.jpg",
                    Price = 1.00m,
                    CategoryId = 2,
                    Quantity = 10
                },

                new Product
                {
                    Id = 5,
                    ProductName = "Cannula",
                    Description = "Tube used for various purposes.",
                    ImageUrl = "https://www.helmed.bg/media/t44s4/1883.webp",
                    Price = 1.73m,
                    CategoryId = 4,
                    Quantity = 10
                },

                new Product
                {
                    Id = 6,
                    ProductName = "Endoscope",
                    Description = "Inspection instrument used to look deep into the body.",
                    ImageUrl = "https://www.msschippers.com/products/images/00/0010816/0010816_fotodtp_1_750x750_1459677832333.jpg",
                    Price = 105.20m,
                    CategoryId = 5,
                    Quantity = 10
                },

                new Product
                {
                    Id = 7,
                    ProductName = "Gas cylinder",
                    Description = "Storage and containment of gasses.",
                    ImageUrl = "https://www.amcaremed.com/wp-content/uploads/2020/01/steel-seamless-medical-gas-cylinder.jpg",
                    Price = 55m,
                    CategoryId = 7,
                    Quantity = 10
                },

                new Product
                {
                    Id = 8,
                    ProductName = "Otoscope",
                    Description = "Medical device used to look into ears.",
                    ImageUrl = "https://m.media-amazon.com/images/I/31W7wpCID4L.jpg",
                    Price = 13.20m,
                    CategoryId = 5,
                    Quantity = 10
                },

                new Product
                {
                    Id = 9,
                    ProductName = "Stethoscope",
                    Description = "Used for auscultation - listening to inner sounds of the human body",
                    ImageUrl = "https://www.veterinarna-apteka.com/images/products/dc02e706cde3923b65404ac663791616.jpg",
                    Price = 20m,
                    CategoryId = 1,
                    Quantity = 10
                },

                new Product
                {
                    Id = 10,
                    ProductName = "Thermometer",
                    Description = "Used for measuring temperature.",
                    ImageUrl = "https://tfa.bg/userfiles/productlargeimages/product_1820.jpg",
                    Price = 3.70m,
                    CategoryId = 5,
                    Quantity = 10
                },

                new Product
                {
                    Id = 11,
                    ProductName = "Transfusion kit",
                    Description = "Used for blood transfusion.",
                    ImageUrl = "https://www.smd-medical.com/wp-content/uploads/2017/01/7-Blood-Transfusion-Set-480x480.jpg",
                    Price = 12.30m,
                    CategoryId = 2,
                    Quantity = 10
                },

                new Product
                {
                    Id = 12,
                    ProductName = "Nebulizer",
                    Description = "Used to deliver medicine in the form of a mist inhaled orally.",
                    ImageUrl = "https://medicaldepot.com.ph/wp-content/uploads/products/3b9a64f6-38fa-fdfc-cadb-74e041fda45f.jpg",
                    Price = 40.20m,
                    CategoryId = 5,
                    Quantity = 10
                },

            };

            return products;
        }
    }
}

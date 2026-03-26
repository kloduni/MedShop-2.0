using MedShop.Core.Contracts;
using System.ComponentModel.DataAnnotations;

namespace MedShop.Core.Models.Product
{
    public class ProductServiceModel : IProductModel
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = null!;

        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; } = null!;

        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        public string Category { get; set; } = null!;

        public int Quantity { get; set; }

        public string Seller { get; set; } = null!;

        public string SellerId { get; set; } = string.Empty;

        public bool IsVisible { get; set; }
        public bool IsInWishlist { get; set; }
        public double AverageRating { get; set; }
        public IEnumerable<ReviewServiceModel> Reviews { get; set; } = new List<ReviewServiceModel>();
        public bool HasPurchased { get; set; }
    }
}

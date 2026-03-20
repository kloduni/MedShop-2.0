using System.ComponentModel.DataAnnotations;
using MedShop.Core.Contracts;

namespace MedShop.Core.Models.Product
{
    public class ProductBaseModel : IProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Product name")]
        public string ProductName { get; set; } = null!;

        [Required]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [Range(0, 999999)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0, 999999)]
        public int Quantity { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<ProductCategoryModel> ProductCategories { get; set; } = new List<ProductCategoryModel>();
    }
}

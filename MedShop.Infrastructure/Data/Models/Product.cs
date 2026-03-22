using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MedShop.Infrastructure.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required] 
        [StringLength(200)] 
        public string Description { get; set; } = null!;

        public int Quantity { get; set; } = 0;

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Soft-delete flag: products are never physically removed so that historical
        // order records can still reference them.  Inactive products are hidden from the
        // shop but remain visible in the admin "deleted products" view for restoration.
        public bool IsActive { get; set; } = true;

        public ICollection<UserProduct> UsersProducts { get; set; } = new List<UserProduct>();
    }
}

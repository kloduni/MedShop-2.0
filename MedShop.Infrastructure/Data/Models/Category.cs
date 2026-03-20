using System.ComponentModel.DataAnnotations;

namespace MedShop.Infrastructure.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

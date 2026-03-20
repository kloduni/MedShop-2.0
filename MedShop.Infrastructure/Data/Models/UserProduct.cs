using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedShop.Infrastructure.Data.Models
{
    public class UserProduct
    {
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public User User { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}

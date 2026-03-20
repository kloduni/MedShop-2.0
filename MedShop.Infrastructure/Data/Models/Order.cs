using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedShop.Infrastructure.Data.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public User User { get; set; } = null!;

        public IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

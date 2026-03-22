using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedShop.Infrastructure.Data.Models
{
    /// <summary>
    /// Join entity for the many-to-many relationship between <see cref="User"/> (acting as seller)
    /// and <see cref="Product"/>.  The composite primary key (UserId + ProductId) is configured in
    /// <c>ApplicationDbContext.OnModelCreating</c>.
    /// </summary>
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

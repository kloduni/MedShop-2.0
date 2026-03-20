using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MedShop.Infrastructure.Data.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int Amount { get; set; }

        [Range(0, 999999)]
        public decimal Price { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;
    }
}

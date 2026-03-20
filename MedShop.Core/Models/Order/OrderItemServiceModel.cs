namespace MedShop.Core.Models.Order
{
    public class OrderItemServiceModel
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = null!;

        public int Amount { get; set; }

        public decimal Price { get; set; }
    }
}

namespace MedShop.Core.Models.Order
{
    public class OrderServiceModel
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string TotalPrice { get; set; } = null!;

        public IEnumerable<OrderItemServiceModel> OrderItems { get; set; } = new List<OrderItemServiceModel>();
    }
}

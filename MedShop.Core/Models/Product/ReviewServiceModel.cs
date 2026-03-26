namespace MedShop.Core.Models.Product
{
    public class ReviewServiceModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string CreatedOn { get; set; } = string.Empty;
    }
}
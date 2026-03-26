using System.ComponentModel.DataAnnotations;

namespace MedShop.Core.Models.Product
{
    public class ReviewFormModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please provide a title.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 50 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide a description.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Review must be between 10 and 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }
    }
}
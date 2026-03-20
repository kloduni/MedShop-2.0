using MedShop.Core.Models.Product;
using MedShop.Core.Models.Product.ProductSortingEnum;

namespace MedShop.Models
{
    public class AllProductsQueryModel
    {
        public const int ProductsPerPage = 6;

        public string? Category { get; set; }

        public string? SearchTerm { get; set; }

        public ProductSorting Sorting { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalProductsCount { get; set; }

        public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();

        public IEnumerable<ProductServiceModel> Products { get; set; } = Enumerable.Empty<ProductServiceModel>();
    }
}

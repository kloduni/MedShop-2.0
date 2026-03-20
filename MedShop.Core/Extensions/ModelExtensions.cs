using System.Text;
using MedShop.Core.Contracts;

namespace MedShop.Core.Extensions
{
    public static class ModelExtensions
    {
        public static string GetInformation(this IProductModel product)
        {
            StringBuilder info = new StringBuilder();

            info.Append("-info:");
            info.Append($"{product.Id * 3}{product.Id * 7}{product.Id * 2}{product.Id * 3}{product.Id}");

            return info.ToString();
        }
    }
}

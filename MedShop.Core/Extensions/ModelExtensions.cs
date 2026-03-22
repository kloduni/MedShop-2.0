using MedShop.Core.Contracts;
using System.Text;

namespace MedShop.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IProductModel"/> used to generate and validate
    /// URL route segments that contain encoded product information.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Builds a URL-safe information string derived from the product ID.
        /// This string is embedded in product detail/edit/delete routes and compared on
        /// the receiving action to prevent URL tampering — if the information segment does
        /// not match, the request is rejected as a potential manipulation attempt.
        /// </summary>
        public static string GetInformation(this IProductModel product)
        {
            StringBuilder info = new StringBuilder();

            info.Append("-info:");
            info.Append($"{product.Id * 3}{product.Id * 7}{product.Id * 2}{product.Id * 3}{product.Id}");

            return info.ToString();
        }
    }
}

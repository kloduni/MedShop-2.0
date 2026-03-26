using MedShop.Core.Contracts;
using System.Text;

namespace MedShop.Core.Extensions
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Builds a URL-safe information string derived from the product ID.
        /// </summary>
        public static string GetInformation(this IProductModel product)
        {
            return GetInformationFromId(product.Id);
        }

        /// <summary>
        /// Overload to generate the information string directly from a raw ID.
        /// Useful when working with Database Entities that don't implement IProductModel.
        /// </summary>
        public static string GetInformationFromId(int productId)
        {
            StringBuilder info = new StringBuilder();

            info.Append("-info:");
            info.Append($"{productId * 3}{productId * 7}{productId * 2}{productId * 3}{productId}");

            return info.ToString();
        }
    }
}
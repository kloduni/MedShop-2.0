using MedShop.Core.Models.Product;

namespace MedShop.Core.Contracts
{
    public interface IWishlistService
    {
        /// <summary>
        /// Toggles a product in the user's wishlist. 
        /// Returns true if it was added, false if it was removed.
        /// </summary>
        Task<bool> ToggleWishlistAsync(int productId, string userId);

        /// <summary>
        /// Checks if a product is already in the user's wishlist.
        /// </summary>
        Task<bool> IsInWishlistAsync(int productId, string userId);

        /// <summary>
        ///Asynchronously retrieves the list of products in the specified user's wishlist.
        /// <param name="userId">The unique identifier of the user whose wishlist is to be retrieved. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of products in the
        /// user's wishlist. The collection is empty if the user has no wishlist items.</returns>
        /// </summary>
        Task<IEnumerable<ProductServiceModel>> GetUserWishlistAsync(string userId);
    }
}
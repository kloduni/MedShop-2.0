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
    }
}
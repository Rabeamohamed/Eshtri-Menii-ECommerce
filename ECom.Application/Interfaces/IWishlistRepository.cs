

using ECom.Application.DTO.Wishlist;
using ECom.Core.Entities.Product;

namespace ECom.Application.Interfaces
{
    public interface IWishlistRepository
    {
        // Get all wishlist items for a user
        Task<IReadOnlyList<WishlistDto>> GetUserWishlistAsync(string userId);
        // Check if product already in wishlist
        Task<bool> IsProductInWishlistAsync (int productId, string userId);
        // Get single wishlist item
        Task<Wishlist> GetWishlistItemAsync(int productId, string userId);

        // Add item to wishlist
        Task AddToWishlistAsync(Wishlist wishlist);

        // Remove item from wishlist
        Task RemoveFromWishlistAsync(Wishlist wishlist);
    }
}

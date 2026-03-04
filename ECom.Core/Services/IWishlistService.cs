

using ECom.Core.DTO.Wishlist;
using ECom.Core.Sharing;

namespace ECom.Core.Services
{
    public interface IWishlistService
    {

        // Get all wishlist items for user
        Task<IReadOnlyList<WishlistDto>> GetUserWishlistAsync(string userId);

        // Add product to wishlist
        Task<ResponseAPI> AddToWishlistAsync(AddWishlistDto dto, string userId);

        // Remove product from wishlist
        Task<ResponseAPI> RemoveFromWishlistAsync(int productId, string userId);

        // Move wishlist item to basket
        Task<ResponseAPI> MoveToBasketAsync(int productId, string userId, string basketId);
    }
}

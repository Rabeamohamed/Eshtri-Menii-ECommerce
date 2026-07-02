using ECom.Application.DTO.Review;
using ECom.Core.Entities.Product;

namespace ECom.Application.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<IReadOnlyList<ReviewDto>> GetProductReviewsAsync(int productId);
        Task<Review> GetReviewByIdAsync(int reviewId);
        Task<bool> HasUserReviewedProductAsync(int productId, string userId);
        Task<bool> HasUserPurchasedProductAsync(int productId, string userEmail);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
    }
}

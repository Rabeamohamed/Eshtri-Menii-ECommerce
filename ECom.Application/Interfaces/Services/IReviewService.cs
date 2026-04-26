using ECom.Application.DTO.Review;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services
{
    public interface IReviewService
    {
        // Get all reviews for a product
        Task<IReadOnlyList<ReviewDto>> GetProductReviewsAsync(int productId);

        // Add a new review
        Task<ResponseAPI> AddReviewAsync(CreateReviewDto dto, string userId, string userEmail);

        // Edit existing review — only owner can edit
        Task<ResponseAPI> UpdateReviewAsync(UpdateReviewDto dto, string userId);

        // Delete review — only owner can delete
        Task<ResponseAPI> DeleteReviewAsync(int reviewId, string userId);
    }
}

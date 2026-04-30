using AutoMapper;
using ECom.Application.DTO.Review;
using ECom.Core.Entities.Product;
using ECom.Application.Sharing;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;

namespace ECom.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            return await _unitOfWork.ReviewRepository.GetProductReviewsAsync(productId);
        }

        public async Task<ResponseAPI> AddReviewAsync(CreateReviewDto dto, string userId, string userEmail)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
            if (product is null)
            {
                return new ResponseAPI(404, "Product not found");
            }

            var hasPurchased = await _unitOfWork.ReviewRepository.HasUserPurchasedProductAsync(dto.ProductId, userEmail);
            if (!hasPurchased)
            {
                return new ResponseAPI(400, "You must purchase the product before reviewing it");
            }

            var hasReviewed = await _unitOfWork.ReviewRepository.HasUserReviewedProductAsync(dto.ProductId, userId);
            if (hasReviewed)
            {
                return new ResponseAPI(400, "You have already reviewed this product");
            }

            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.IsVerifiedPurchase = true;
            review.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.ReviewRepository.AddReviewAsync(review);
            await RecalculateProductRatingAsync(dto.ProductId);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new ResponseAPI(400, "Failed to add review");
            }
            return new ResponseAPI(201, "Review added successfully");
        }

        public async Task<ResponseAPI> UpdateReviewAsync(UpdateReviewDto dto, string userId)
        {
            var review = await _unitOfWork.ReviewRepository.GetReviewByIdAsync(dto.ReviewId);
            if (review is null)
                return new ResponseAPI(404, "Review not found");

            if (review.UserId != userId)
                return new ResponseAPI(403, "You are not allowed to update this review");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ReviewRepository.UpdateReviewAsync(review);
            await RecalculateProductRatingAsync(review.ProductId);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new ResponseAPI(400, "Failed to update review");
            }
            return new ResponseAPI(200, "Review Updated successfully");
        }

        public async Task<ResponseAPI> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _unitOfWork.ReviewRepository.GetReviewByIdAsync(reviewId);
            if (review is null)
                return new ResponseAPI(404, "Review not found");

            if (review.UserId != userId)
                return new ResponseAPI(403, "You are not allowed to delete this review");

            var productId = review.ProductId;

            await _unitOfWork.ReviewRepository.DeleteReviewAsync(review);
            await RecalculateProductRatingAsync(productId);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
                return new ResponseAPI(400, "Failed to delete review");

            return new ResponseAPI(200, "Review deleted successfully");
        }

        private async Task RecalculateProductRatingAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId, p => p.Reviews);

            if (product is null)
                return;

            product.TotalReviews = product.Reviews.Count;
            product.AverageRating = product.TotalReviews > 0
                ? Math.Round(product.Reviews.Average(r => r.Rating), 1)
                : 0.0;
            await _unitOfWork.ProductRepository.UpdateAsync(product);
        }
    }
}

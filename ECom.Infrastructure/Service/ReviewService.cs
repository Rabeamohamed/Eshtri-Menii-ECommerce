using AutoMapper;
using ECom.Application.DTO.Review;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Application.Interfaces;
using ECom.Application.Services;
using ECom.Application.Sharing;

namespace ECom.Infrastructure.Service
{
    internal class ReviewService : IReviewService
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
            //1 check if product is exists
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
            if(product is null)
            {
                return new ResponseAPI(404, "Product not found");
            }

            //2 Check if user purchased the product
            var hasPurchased = await _unitOfWork.ReviewRepository.HasUserPurchasedProductAsync(dto.ProductId,userEmail);
            if(!hasPurchased)
            {
                return new ResponseAPI(400, "You must purchase the product before reviewing it");
            }

            // 3. Check if user already reviewed this product
            var hasReviewed = await _unitOfWork.ReviewRepository.HasUserReviewedProductAsync(dto.ProductId, userId);
            if(hasReviewed)
            {
                return new ResponseAPI(400, "You have already reviewed this product");
            }

            // 4. Map DTO → Entity and set fields
            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.IsVerifiedPurchase = true;
            review.CreatedAt = DateTime.UtcNow;

            // 5. Add Review
            await _unitOfWork.ReviewRepository.AddReviewAsync(review);

            // 6. Recalculate AverageRating on Product
            await RecalculateProductRatingAsync(dto.ProductId);

            // 7. Save Everything in one transaction
            var result = await _unitOfWork.SaveChangesAsync();
            if(result <= 0)
            {
                return new ResponseAPI(400, "Failed to add review");
                
            }
            return new ResponseAPI(201, "Review added successfully");
        }

        public async Task<ResponseAPI> UpdateReviewAsync(UpdateReviewDto dto, string userId)
        {
            //1 Get review
            var review = await _unitOfWork.ReviewRepository.GetReviewByIdAsync(dto.ReviewId);
            if(review is null)
                return new ResponseAPI(404, "Review not found");

            // 2 Check if the review belongs to the user
            if(review.UserId != userId)
                return new ResponseAPI(403, "You are not allowed to update this review");

            // 3 Update fields
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            // 4 Update Review
            await _unitOfWork.ReviewRepository.UpdateReviewAsync(review);

            // 5. Recalculate AverageRating on Product
            await RecalculateProductRatingAsync(review.ProductId);

            var result =  await _unitOfWork.SaveChangesAsync();
            if(result <= 0)
            {
                return new ResponseAPI(400, "Failed to update review");
            }
            return new ResponseAPI(200, "Review Updated successfully");
        }

        public async Task<ResponseAPI> DeleteReviewAsync(int reviewId, string userId)
        {
            //1 Get review
            var review = await _unitOfWork.ReviewRepository.GetReviewByIdAsync(reviewId);
            if (review is null)
                return new ResponseAPI(404, "Review not found");

            // 2 Check if the review belongs to the user
            if (review.UserId != userId)
                return new ResponseAPI(403, "You are not allowed to update this review");

            // 3. Store productId before deleting
            var productId = review.ProductId;

            //4 Delete review
            await _unitOfWork.ReviewRepository.DeleteReviewAsync(review);

            // 5. Recalculate AverageRating on Product
            await RecalculateProductRatingAsync(productId);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
                return new ResponseAPI(400, "Failed to delete review");

            return new ResponseAPI(200, "Review deleted successfully");
        }

        // Private helper method for recalculates and updates AverageRating on Product
        private async Task RecalculateProductRatingAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId,p =>p.Reviews);

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

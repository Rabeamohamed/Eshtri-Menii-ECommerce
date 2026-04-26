
using AutoMapper;
using ECom.Application.DTO.Review;
using ECom.Core.Entities.Product;
using ECom.Application.Interfaces;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECom.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ReviewRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            return Task.CompletedTask;
        }

        public Task DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
        }

        public async Task<Core.Entities.Product.Review> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<bool> HasUserPurchasedProductAsync(int productId, string userEmail)
        {
            return await _context.Orders
                .Where(o => o.BuyerEmail == userEmail)
                .SelectMany(o => o.OrderItems)
                .AnyAsync(i => i.ProductItemId == productId);
            // Check if the user has purchased the product before allowing them to review
        }

        public async Task<bool> HasUserReviewedProductAsync(int productId, string userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.ProductId == productId && r.UserId == userId); 
            // Check if the user has already reviewed the product
        }

        public Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            return Task.CompletedTask;
        }
    }
}

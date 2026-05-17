using ECom.Application.DTO.Review;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    [Authorize]
    public class ReviewsController : BaseController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [AllowAnonymous]
        [HttpGet("get-product-reviews/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviewsAsync(productId);
            return Ok(reviews);
        }

        [HttpPost("add-review")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto? dto)
        {
            if (dto is null)
                return BadRequest(new ResponseAPI(400, "Review data is required."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userId is null || userEmail is null)
                return Unauthorized(new ResponseAPI(401, "Not authenticated."));

            var result = await _reviewService.AddReviewAsync(dto, userId, userEmail);

            return result.StatusCode switch
            {
                201 => StatusCode(201, result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        [HttpPut("update-review")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDto? dto)
        {
            if (dto is null)
                return BadRequest(new ResponseAPI(400, "Review data is required."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized(new ResponseAPI(401, "Not authenticated."));

            var result = await _reviewService.UpdateReviewAsync(dto, userId);

            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                403 => StatusCode(403, result),
                _ => BadRequest(result)
            };
        }

        [HttpDelete("delete-review/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized(new ResponseAPI(401, "Not authenticated."));

            var result = await _reviewService.DeleteReviewAsync(reviewId, userId);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                403 => StatusCode(403, result),
                _ => BadRequest(result)
            };
        }
    }
}

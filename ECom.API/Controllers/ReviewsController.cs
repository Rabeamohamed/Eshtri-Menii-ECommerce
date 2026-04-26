
using ECom.Application.DTO.Review;
using ECom.Application.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
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
            try
            {
                var reviews = await _reviewService.GetProductReviewsAsync(productId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpPost("add-review")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Assuming the user ID is stored in the NameIdentifier claim
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value; // Assuming the user email is stored in the Email claim
                
                if (userId is null || userEmail is null)
                {
                    return Unauthorized(new ResponseAPI(401));
                }
                var result = await _reviewService.AddReviewAsync(dto, userId, userEmail);

                return result.StatusCode switch
                {
                    201 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpPut("update-review")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Assuming the user ID is stored in the NameIdentifier claim
                
                if (userId is null )
                {
                    return Unauthorized(new ResponseAPI(401));
                }
                var result = await _reviewService.UpdateReviewAsync(dto, userId);

                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    403 => StatusCode(403, result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpDelete("delete-review/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Assuming the user ID is stored in the NameIdentifier claim

                if (userId is null)
                {
                    return Unauthorized(new ResponseAPI(401));
                }
                var result = await _reviewService.DeleteReviewAsync(reviewId, userId);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    403 => StatusCode(403, result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpGet("get-claims")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
    }
}

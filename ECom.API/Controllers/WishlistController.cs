using ECom.Application.DTO.Wishlist;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    [Authorize]
    public class WishlistController : BaseController
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet("get-wishlist")]
        public async Task<IActionResult> GetUserWishlist()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized(new ResponseAPI(401, "Not authenticated."));

            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto? dto)
        {
            if (dto is null)
                return BadRequest(new ResponseAPI(400, "Request body is required."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized(new ResponseAPI(401, "Not authenticated."));

            var response = await _wishlistService.AddToWishlistAsync(dto, userId);
            return response.StatusCode switch
            {
                201 => StatusCode(201, response),
                404 => NotFound(response),
                _ => BadRequest(response)
            };
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized(new ResponseAPI(401, "Not authenticated."));

            var response = await _wishlistService.RemoveFromWishlistAsync(productId, userId);
            return response.StatusCode switch
            {
                200 => Ok(response),
                404 => NotFound(response),
                403 => StatusCode(403, response),
                _ => BadRequest(response)
            };
        }

        [HttpPost("move-to-basket")]
        public async Task<IActionResult> MoveToBasket([FromBody] MoveToBasketDto? dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.BasketId))
                return BadRequest(new ResponseAPI(400, "Product and basket id are required."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized(new ResponseAPI(401, "Not authenticated."));

            var result = await _wishlistService.MoveToBasketAsync(dto.ProductId, userId, dto.BasketId);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
    }
}

using ECom.Application.DTO.Wishlist;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // GET: api/wishlist/get-wishlist
        [HttpGet("get-wishlist")]
        public async Task<IActionResult> GetUserWishlist()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                    return Unauthorized(new ResponseAPI(401));

                var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // POST: api/wishlist/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                    return Unauthorized(new ResponseAPI(401));

                var response = await _wishlistService.AddToWishlistAsync(dto, userId);
                return response.StatusCode switch
                {
                    201 => Ok(response),
                    404 => NotFound(response),
                    _ => BadRequest(response)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }

        }

        // DELETE: api/wishlist/remove/{productId}
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                    return Unauthorized(new ResponseAPI(401));

                var response = await _wishlistService.RemoveFromWishlistAsync(productId, userId);
                return response.StatusCode switch
                {
                    200 => Ok(response),
                    404 => NotFound(response),
                    403 => StatusCode(403, response),
                    _ => BadRequest(response)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // POST: api/wishlist/move-to-basket
        [HttpPost("move-to-basket")]
        public async Task<IActionResult> MoveToBasket([FromBody] MoveToBasketDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId is null)
                    return Unauthorized(new ResponseAPI(401));

                var result = await _wishlistService
                    .MoveToBasketAsync(dto.ProductId, userId, dto.BasketId);

                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        }
}

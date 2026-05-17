using ECom.Application.Common.Exceptions;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class BasketsController : BaseController
    {
        private readonly IBasketService _basketService;

        public BasketsController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet("get-basket-item/{id}")]
        public async Task<IActionResult> GetBasketItem(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ResponseAPI(400, "Basket id is required"));

            var basket = await _basketService.GetBasketAsync(id);
            if (basket is null)
                return Ok(new CustomerBasket(id) { BasketItems = new List<BasketItem>() });

            return Ok(basket);
        }

        [HttpPost("update-basket")]
        public async Task<IActionResult> UpdateBasketItem([FromBody] CustomerBasket? basket)
        {
            if (basket is null)
                return BadRequest(new ResponseAPI(400, "Basket payload is required"));

            try
            {
                var updatedBasket = await _basketService.UpdateBasketAsync(basket);
                return Ok(updatedBasket);
            }
            catch (BusinessException bex)
            {
                return BadRequest(new ResponseAPI(bex.StatusCode, bex.Message));
            }
        }

        [HttpDelete("delete-basket-item/{id}")]
        public async Task<IActionResult> DeleteBasketItem(string id)
        {
            var result = await _basketService.DeleteBasketAsync(id);
            return result.StatusCode switch
            {
                200 => Ok(result),
                _ => BadRequest(result)
            };
        }
    }
}

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
            var basket = await _basketService.GetBasketAsync(id);
            if (basket == null)
            {
                return Ok(new CustomerBasket());
            }

            return Ok(basket);
        }

        [HttpPost("update-basket")]
        public async Task<IActionResult> UpdateBasketItem(CustomerBasket basket)
        {
            var updatedBasket = await _basketService.UpdateBasketAsync(basket);
            return Ok(updatedBasket);
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

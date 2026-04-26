using AutoMapper;
using ECom.Core.Entities;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;
using ECom.Application.Interfaces.Repositories;

namespace ECom.API.Controllers
{
   
    public class BasketsController : BaseController
    {
        public BasketsController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("get-basket-item/{id}")]
        public async Task<IActionResult> GetBasketItem(string id)
        {
            var basket = await work.CustomerBasketRepository.GetBasketAsync(id);
            if (basket == null)
            {
                return Ok(new CustomerBasket());
            }
            return Ok(basket);
        }
        [HttpPost("update-basket")]
        public async Task<IActionResult> UpdateBasketItem( CustomerBasket basket)
        {
            var updatedBasket = await work.CustomerBasketRepository.UpdateBasketAsync(basket);
            return Ok(updatedBasket);
        }
        [HttpDelete("delete-basket-item/{id}")]
        public async Task<IActionResult> DeleteBasketItem( string id)
        {
            var deleted = await work.CustomerBasketRepository.DeleteBasketAsync(id);
            return deleted? Ok(new ResponseAPI(200, "Item Deleted Successfully")) 
                : BadRequest(new ResponseAPI(400));
        }
    }
}
 
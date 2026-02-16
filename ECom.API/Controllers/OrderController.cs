using ECom.API.Helper;
using ECom.Core.DTO.Order;
using ECom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-order")]
        public async Task<ActionResult> Create(OrderDto orderDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var order = await _orderService.CreateOrderAsync(orderDto, email);

            if (order is null) return BadRequest(new { Message = "Problem creating order" });
            return Ok(order);
            //return Ok(new ResponseAPI(200,order);
        }
    }
}

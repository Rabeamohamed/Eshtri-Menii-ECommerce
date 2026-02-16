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
        }

        [HttpGet("get-orders-for-user")]
        public async Task<IActionResult> GetOrdersFOrUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var orders = _orderService.GetAllOrdersForUserAsync(email);
            return Ok(orders);
        }

        [HttpGet("get-order-by-id")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var order = _orderService.GetOrderByIdAsync(id, email);
            return Ok(order);
        }
        [HttpGet("get-delivery")]
        public async Task<IActionResult> GetDeliver()
        => Ok(await _orderService.GetDeliveryMethodAsync());
        
    }
}

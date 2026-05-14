using AutoMapper;
using ECom.Application.DTO.Order;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("create-order")]
        public async Task<ActionResult> Create(OrderDto orderDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var order = await _orderService.CreateOrderAsync(orderDto, email!);

            if (order is null)
                return BadRequest(new ResponseAPI(400, "Problem creating order"));

            return Ok(_mapper.Map<OrderToReturnDto>(order));
        }

        [HttpGet("get-orders-for-user")]
        public async Task<IActionResult> GetOrdersFOrUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var orders = await _orderService.GetAllOrdersForUserAsync(email!);
            return Ok(orders);
        }

        [HttpGet("get-order-by-id/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var order = await _orderService.GetOrderByIdAsync(id, email!);
            if (order == null)
                return NotFound(new { message = "Order not found" });
            return Ok(order);
        }

        [HttpGet("get-delivery")]
        public async Task<IActionResult> GetDeliver()
            => Ok(await _orderService.GetDeliveryMethodAsync());

        [HttpPut("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _orderService.CancelOrderAsync(orderId, email!, isAdmin: false);

            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                403 => StatusCode(403, result),
                _ => BadRequest(result)
            };
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin-cancel/{orderId}")]
        public async Task<IActionResult> AdminCancelOrder(int orderId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _orderService.CancelOrderAsync(orderId, email!, isAdmin: true);

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

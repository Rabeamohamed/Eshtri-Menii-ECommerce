using AutoMapper;
using ECom.Application.DTO.Order;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    // Customer-only order operations. Only users with the 'Customer' role can create/view orders.
    [Authorize(Roles = "Customer")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        private string? BuyerEmail => User.FindFirst(ClaimTypes.Email)?.Value;

        [HttpPost("create-order")]
        public async Task<ActionResult> Create([FromBody] OrderDto orderDto)
        {
            var email = BuyerEmail;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new ResponseAPI(401, "Authenticated user email is required."));
            if (orderDto is null)
                return BadRequest(new ResponseAPI(400, "Order payload is required."));

            var order = await _orderService.CreateOrderAsync(orderDto, email);

            if (order is null)
                return BadRequest(new ResponseAPI(400, "Problem creating order"));

            return Ok(_mapper.Map<OrderToReturnDto>(order));
        }

        [HttpGet("get-orders-for-user")]
        public async Task<IActionResult> GetOrdersForUser()
        {
            var email = BuyerEmail;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new ResponseAPI(401, "Authenticated user email is required."));

            var orders = await _orderService.GetAllOrdersForUserAsync(email);
            return Ok(orders);
        }

        [HttpGet("get-order-by-id/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var email = BuyerEmail;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new ResponseAPI(401, "Authenticated user email is required."));

            var order = await _orderService.GetOrderByIdAsync(id, email);
            if (order is null)
                return NotFound(new ResponseAPI(404, "Order not found"));
            return Ok(order);
        }

        [HttpGet("get-delivery")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDeliver()
            => Ok(await _orderService.GetDeliveryMethodAsync());

        [HttpPut("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var email = BuyerEmail;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new ResponseAPI(401, "Authenticated user email is required."));

            var result = await _orderService.CancelOrderAsync(orderId, email, isAdmin: false);

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

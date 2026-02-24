using AutoMapper;
using ECom.Core.DTO.Order;
using ECom.Core.Services;
using ECom.Core.Sharing;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IMapper _mapper; // ✅ add this
        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("create-order")]
        public async Task<ActionResult> Create(OrderDto orderDto)
        {
            try
            {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var order = await _orderService.CreateOrderAsync(orderDto, email);

                if (order is null)
                    return BadRequest(new ResponseAPI(400, "Problem creating order"));

                return Ok(_mapper.Map<OrderToReturnDto>(order));
            }
            catch (Exception ex)
            {
                // Stock error will be returned here
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpGet("get-orders-for-user")]
        public async Task<IActionResult> GetOrdersFOrUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var orders = await _orderService.GetAllOrdersForUserAsync(email);
            return Ok(orders);
        }

        [HttpGet("get-order-by-id/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var order = await _orderService.GetOrderByIdAsync(id, email);
            if (order == null)
                return NotFound(new { message = "Order not found" });
            return Ok(order);
        }
        [HttpGet("get-delivery")]
        public async Task<IActionResult> GetDeliver()
        => Ok(await _orderService.GetDeliveryMethodAsync());
        
    }
}

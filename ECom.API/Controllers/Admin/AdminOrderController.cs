using ECom.Core.Entities.Order;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;
using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Interfaces.Services;

namespace ECom.API.Controllers.Admin
{
    public class AdminOrderController : AdminBaseController
    {
        private readonly IAdminOrderService _orderService;
        private readonly IOrderService _baseOrderService;

        public AdminOrderController(IAdminOrderService orderService, IOrderService baseOrderService)
        {
            _orderService = orderService;
            _baseOrderService = baseOrderService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllOrders([FromQuery] PaymentStatus? status = null)
        {
            var orders = await _orderService.GetAllOrdersAsync(status);
            return Ok(orders);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null)
                return NotFound(new ResponseAPI(404, $"Order not found with Id {id}"));
            return Ok(order);
        }

        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] PaymentStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        [HttpGet("orders-count")]
        public async Task<IActionResult> GetOrdersCountByStatus()
        {
            var counts = await _orderService.GetOrdersCountByStatusAsync();
            return Ok(counts);
        }

        [HttpPut("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var result = await _baseOrderService.CancelOrderAsync(orderId, buyerEmail: null!, isAdmin: true);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }
    }
}

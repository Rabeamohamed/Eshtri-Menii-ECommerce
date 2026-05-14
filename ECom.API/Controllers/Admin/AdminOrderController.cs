using ECom.Core.Entities.Order;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;
using ECom.Application.Interfaces.Services.Admin;

namespace ECom.API.Controllers.Admin
{
    public class AdminOrderController : AdminBaseController
    {
        private readonly IAdminOrderService _orderService;
        public AdminOrderController(IAdminOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/admin/adminorder/get-all
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllOrders([FromQuery] PaymentStatus? status = null)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminorder/get-by-id/1
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order is null)
                    return NotFound(new ResponseAPI(404, $"Order not found with Id {id}"));
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // PUT: api/admin/adminorder/update-status/1
        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] PaymentStatus status)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
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

        // GET: api/admin/adminorder/orders-count
        [HttpGet("orders-count")]
        public async Task<IActionResult> GetOrdersCountByStatus()
        {
            try
            {
                var counts = await _orderService.GetOrdersCountByStatusAsync();
                return Ok(counts);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
    }
}

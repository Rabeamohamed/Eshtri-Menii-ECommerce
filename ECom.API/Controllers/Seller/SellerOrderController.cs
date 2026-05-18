using ECom.Application.DTO.Order;
using ECom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers.Seller
{
    public class SellerOrderController : SellerBaseController
    {
        private readonly ISellerOrderService _orderService;

        public SellerOrderController(ISellerOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetSellerId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetMyOrders()
        {
            var sellerId = GetSellerId();
            var orders = await _orderService.GetMyOrdersAsync(sellerId);
            return Ok(orders);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetMyOrderById(int id)
        {
            var sellerId = GetSellerId();
            var order = await _orderService.GetMyOrderByIdAsync(sellerId, id);
            
            if (order is null)
                return NotFound(new { Message = "Order not found or contains no items belonging to you." });
                
            return Ok(order);
        }
    }
}

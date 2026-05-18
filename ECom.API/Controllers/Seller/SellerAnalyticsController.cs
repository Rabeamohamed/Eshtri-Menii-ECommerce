using ECom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers.Seller
{
    public class SellerAnalyticsController : SellerBaseController
    {
        private readonly ISellerAnalyticsService _analyticsService;

        public SellerAnalyticsController(ISellerAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        private string GetSellerId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var sellerId = GetSellerId();
            var summary = await _analyticsService.GetMyDashboardSummaryAsync(sellerId);
            return Ok(summary);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            var sellerId = GetSellerId();
            var revenue = await _analyticsService.GetMyRevenueAsync(sellerId);
            return Ok(revenue);
        }

        [HttpGet("order-stats")]
        public async Task<IActionResult> GetOrderStats()
        {
            var sellerId = GetSellerId();
            var stats = await _analyticsService.GetMyOrderStatsAsync(sellerId);
            return Ok(stats);
        }

        [HttpGet("best-selling")]
        public async Task<IActionResult> GetBestSelling([FromQuery] int count = 5)
        {
            var sellerId = GetSellerId();
            var products = await _analyticsService.GetMyBestSellingProductsAsync(sellerId, count);
            return Ok(products);
        }

        [HttpGet("out-of-stock")]
        public async Task<IActionResult> GetOutOfStock()
        {
            var sellerId = GetSellerId();
            var products = await _analyticsService.GetMyOutOfStockProductsAsync(sellerId);
            return Ok(products);
        }
    }
}

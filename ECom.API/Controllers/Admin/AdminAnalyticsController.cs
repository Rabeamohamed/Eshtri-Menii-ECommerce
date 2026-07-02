using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{

    public class AdminAnalyticsController : AdminBaseController
    {
        private readonly IAdminAnalyticsService _analyticsService;

        public AdminAnalyticsController(IAdminAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // GET: api/admin/adminanalytics/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var summary = await _analyticsService.GetDashboardSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminanalytics/revenue
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            try
            {
                var revenue = await _analyticsService.GetRevenueAsync();
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminanalytics/order-stats
        [HttpGet("order-stats")]
        public async Task<IActionResult> GetOrderStats()
        {
            try
            {
                var stats = await _analyticsService.GetOrderStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminanalytics/best-selling
        [HttpGet("best-selling")]
        public async Task<IActionResult> GetBestSellingProducts([FromQuery] int count = 10)
        {
            try
            {
                var products = await _analyticsService.GetBestSellingProductsAsync(count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminanalytics/out-of-stock
        [HttpGet("out-of-stock")]
        public async Task<IActionResult> GetOutOfStockProducts()
        {
            try
            {
                var products = await _analyticsService.GetOutOfStockProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminanalytics/top-customers
        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int count = 10)
        {
            try
            {
                var customers = await _analyticsService.GetTopCustomersAsync(count);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminanalytics/category-sales
        [HttpGet("category-sales")]
        public async Task<IActionResult> GetCategorySales()
        {
            try
            {
                var sales = await _analyticsService.GetCategorySalesAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
    }
}

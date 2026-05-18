using ECom.Application.DTO.Admin;
using ECom.Application.Interfaces.Repositories;
using ECom.Core.Entities.Order;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECom.Infrastructure.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppDbContext _context;

        public AnalyticsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<BestSellingProductDto>> GetBestSellingProductsAsync(int count)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.Status == PaymentStatus.PaymentReceived)
                .GroupBy(oi => new { oi.ProductItemId, oi.ProductName })
                .Select(g => new BestSellingProductDto
                {
                    ProductId = g.Key.ProductItemId,
                    ProductName = g.Key.ProductName,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<CategorySalesDto>> GetCategorySalesAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.Status == PaymentStatus.PaymentReceived)
                .Join(_context.Products.Include(p => p.Category),
                    oi => oi.ProductItemId,
                    p => p.Id,
                    (oi, p) => new { oi, p })
                .GroupBy(x => new { x.p.CategoryId, CategoryName = x.p.Category.Name })
                .Select(g => new CategorySalesDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalProductsSold = g.Sum(x => x.oi.Quantity),
                    TotalRevenue = g.Sum(x => x.oi.Price * x.oi.Quantity)
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToListAsync();
        }

        public async Task<OrderStatsDto> GetOrderStatsAsync()
        {
            var orders = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return new OrderStatsDto
            {
                TotalOrders = orders.Sum(s => s.Count),
                PendingOrders = orders
                    .FirstOrDefault(s => s.Status == PaymentStatus.Pending)?.Count ?? 0,
                CompletedOrders = orders
                    .FirstOrDefault(s => s.Status == PaymentStatus.PaymentReceived)?.Count ?? 0,
                CancelledOrders = orders
                    .FirstOrDefault(s => s.Status == PaymentStatus.Cancelled)?.Count ?? 0,
                FailedOrders = orders
                    .FirstOrDefault(s => s.Status == PaymentStatus.PaymentFailed)?.Count ?? 0
            };
        }

        public async Task<RevenueDto> GetRevenueAsync()
        {
            var query = _context.Orders
                .Where(o => o.Status == PaymentStatus.PaymentReceived);

            var now = DateTime.UtcNow;
            
            var totalRevenue = await query.SumAsync(o => o.SubTotal);
            var dailyRevenue = await query
                .Where(o => o.OrderDate >= now.AddDays(-1))
                .SumAsync(o => o.SubTotal);
            var weeklyRevenue = await query
                .Where(o => o.OrderDate >= now.AddDays(-7))
                .SumAsync(o => o.SubTotal);
            var monthlyRevenue = await query
                .Where(o => o.OrderDate >= now.AddMonths(-1))
                .SumAsync(o => o.SubTotal);
            
            var orderCount = await query.CountAsync();
            var averageOrderValue = orderCount > 0
                ? totalRevenue / orderCount
                : 0;

            return new RevenueDto
            {
                TotalRevenue = totalRevenue,
                DailyRevenue = dailyRevenue,
                WeeklyRevenue = weeklyRevenue,
                MonthlyRevenue = monthlyRevenue,
                AverageOrderValue = Math.Round(averageOrderValue, 2)
            };
        }

        public async Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersAsync(int count)
        {
            return await _context.Orders
                .Where(o => o.Status == PaymentStatus.PaymentReceived)
                .GroupBy(o => o.BuyerEmail)
                .Select(g => new TopCustomerDto
                {
                    Email = g.Key,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.SubTotal)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<BestSellingProductDto>> GetOutOfStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity == 0)
                .Select(p => new BestSellingProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    TotalSold = 0,
                    TotalRevenue = 0,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        // Seller Specific Implementations
        public async Task<IReadOnlyList<BestSellingProductDto>> GetSellerBestSellingProductsAsync(string sellerId, int count)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.SellerId == sellerId && oi.Order.Status == PaymentStatus.PaymentReceived)
                .GroupBy(oi => new { oi.ProductItemId, oi.ProductName })
                .Select(g => new BestSellingProductDto
                {
                    ProductId = g.Key.ProductItemId,
                    ProductName = g.Key.ProductName,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(count)
                .ToListAsync();
        }

        public async Task<OrderStatsDto> GetSellerOrderStatsAsync(string sellerId)
        {
            var sellerItems = _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.SellerId == sellerId);

            // Total orders that contain at least one item from this seller
            var totalOrders = await sellerItems.Select(oi => oi.OrderId).Distinct().CountAsync();
            var pendingOrders = await sellerItems.Where(oi => oi.Order.Status == PaymentStatus.Pending).Select(oi => oi.OrderId).Distinct().CountAsync();
            var completedOrders = await sellerItems.Where(oi => oi.Order.Status == PaymentStatus.PaymentReceived).Select(oi => oi.OrderId).Distinct().CountAsync();

            return new OrderStatsDto
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders
            };
        }

        public async Task<RevenueDto> GetSellerRevenueAsync(string sellerId)
        {
            var paidItems = _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.SellerId == sellerId && oi.Order.Status == PaymentStatus.PaymentReceived);

            var totalRevenue = await paidItems.SumAsync(oi => oi.Price * oi.Quantity);

            var thisMonth = DateTime.Now.Month;
            var thisYear = DateTime.Now.Year;

            var monthlyRevenue = await paidItems
                .Where(oi => oi.Order.OrderDate.Month == thisMonth && oi.Order.OrderDate.Year == thisYear)
                .SumAsync(oi => oi.Price * oi.Quantity);

            return new RevenueDto
            {
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue
            };
        }

        public async Task<IReadOnlyList<BestSellingProductDto>> GetSellerOutOfStockProductsAsync(string sellerId)
        {
            return await _context.Products
                .Where(p => p.SellerId == sellerId && p.StockQuantity <= 0)
                .Select(p => new BestSellingProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    TotalSold = 0,
                    TotalRevenue = 0
                })
                .ToListAsync();
        }
    }
}

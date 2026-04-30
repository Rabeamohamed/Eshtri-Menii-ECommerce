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
            return await _context.Products
                .Include(p => p.Category)
                .GroupBy(p => new { p.CategoryId, p.Category.Name })
                .Select(g => new CategorySalesDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    TotalProductsSold = g.Count(),
                    TotalRevenue = 0 
                })
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
            var completedOrders = await _context.Orders
                .Where(o => o.Status == PaymentStatus.PaymentReceived)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var totalRevenue = completedOrders.Sum(o => o.SubTotal);
            var dailyRevenue = completedOrders
                .Where(o => o.OrderDate >= now.AddDays(-1))
                .Sum(o => o.SubTotal);
            var weeklyRevenue = completedOrders
                .Where(o => o.OrderDate >= now.AddDays(-7))
                .Sum(o => o.SubTotal);
            var monthlyRevenue = completedOrders
                .Where(o => o.OrderDate >= now.AddMonths(-1))
                .Sum(o => o.SubTotal);
            var averageOrderValue = completedOrders.Any()
                ? completedOrders.Average(o => o.SubTotal)
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
    }
}

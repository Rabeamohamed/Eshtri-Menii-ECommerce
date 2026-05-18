using ECom.Application.DTO.Admin;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services.Admin;

namespace ECom.Application.Services.Admin
{
    public class AdminAnalyticsService : IAdminAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminAnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<BestSellingProductDto>> GetBestSellingProductsAsync(int count = 10)
        {
            return await _unitOfWork.AnalyticsRepository.GetBestSellingProductsAsync(count);
        }

        public async Task<IReadOnlyList<CategorySalesDto>> GetCategorySalesAsync()
        {
            return await _unitOfWork.AnalyticsRepository.GetCategorySalesAsync();
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var revenue = await GetRevenueAsync();
            var orderStats = await GetOrderStatsAsync();
            var bestSelling = await GetBestSellingProductsAsync(5);
            var topCustomers = await GetTopCustomersAsync(5);
            var categorySales = await GetCategorySalesAsync();
            var totalProducts = await _unitOfWork.ProductRepository.CountAsync();
            var outOfStock = await GetOutOfStockProductsAsync();
            var totalCategories = await _unitOfWork.CategoryRepository.CountAsync();
            var totalUsers = await _unitOfWork.AnalyticsRepository.GetUserCountAsync();

            return new DashboardSummaryDto
            {
                Revenue = revenue,
                OrderStats = orderStats,
                TotalProducts = totalProducts,
                OutOfStockProducts = outOfStock.Count,
                TotalUsers = totalUsers,
                TotalCategories = totalCategories,
                BestSellingProducts = bestSelling,
                TopCustomers = topCustomers,
                CategorySales = categorySales
            };
        }

        public async Task<OrderStatsDto> GetOrderStatsAsync()
        {
            return await _unitOfWork.AnalyticsRepository.GetOrderStatsAsync();
        }

        public async Task<IReadOnlyList<BestSellingProductDto>> GetOutOfStockProductsAsync()
        {
            return await _unitOfWork.AnalyticsRepository.GetOutOfStockProductsAsync();
        }

        public async Task<RevenueDto> GetRevenueAsync()
        {
            return await _unitOfWork.AnalyticsRepository.GetRevenueAsync();
        }

        public async Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersAsync(int count = 10)
        {
            return await _unitOfWork.AnalyticsRepository.GetTopCustomersAsync(count);
        }
    }
}

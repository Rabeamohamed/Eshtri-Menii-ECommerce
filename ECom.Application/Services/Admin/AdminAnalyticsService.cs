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
            var revenueTask = GetRevenueAsync();
            var orderStatsTask = GetOrderStatsAsync();
            var bestSellingTask = GetBestSellingProductsAsync(5);
            var topCustomersTask = GetTopCustomersAsync(5);
            var categorySalesTask = GetCategorySalesAsync();
            var totalProductsTask = _unitOfWork.ProductRepository.CountAsync();
            var outOfStockTask = GetOutOfStockProductsAsync();
            var totalCategoriesTask = _unitOfWork.CategoryRepository.CountAsync();
            var totalUsersTask = _unitOfWork.AnalyticsRepository.GetUserCountAsync();

            await Task.WhenAll(
                revenueTask, orderStatsTask, bestSellingTask,
                topCustomersTask, categorySalesTask, totalProductsTask,
                outOfStockTask, totalCategoriesTask, totalUsersTask);

            return new DashboardSummaryDto
            {
                Revenue = await revenueTask,
                OrderStats = await orderStatsTask,
                TotalProducts = await totalProductsTask,
                OutOfStockProducts = (await outOfStockTask).Count,
                TotalUsers = await totalUsersTask,
                TotalCategories = await totalCategoriesTask,
                BestSellingProducts = await bestSellingTask,
                TopCustomers = await topCustomersTask,
                CategorySales = await categorySalesTask
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

using ECom.Application.DTO.Admin;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;

namespace ECom.Application.Services
{
    public class SellerAnalyticsService : ISellerAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SellerAnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<BestSellingProductDto>> GetMyBestSellingProductsAsync(string sellerId, int count = 10)
        {
            return await _unitOfWork.AnalyticsRepository.GetSellerBestSellingProductsAsync(sellerId, count);
        }

        public async Task<DashboardSummaryDto> GetMyDashboardSummaryAsync(string sellerId)
        {
            var revenue = await GetMyRevenueAsync(sellerId);
            var orderStats = await GetMyOrderStatsAsync(sellerId);
            var bestSelling = await GetMyBestSellingProductsAsync(sellerId, 5);
            var outOfStock = await GetMyOutOfStockProductsAsync(sellerId);

            // Total products owned by this seller
            var totalProducts = await _unitOfWork.ProductRepository.CountAsync(new Sharing.ProductParams { SellerId = sellerId });

            return new DashboardSummaryDto
            {
                Revenue = revenue,
                OrderStats = orderStats,
                TotalProducts = totalProducts,
                OutOfStockProducts = outOfStock.Count,
                BestSellingProducts = bestSelling,
                // These are not applicable or restricted for sellers, so we leave them default/0
                TotalUsers = 0,
                TotalCategories = 0,
                TopCustomers = new List<TopCustomerDto>(),
                CategorySales = new List<CategorySalesDto>()
            };
        }

        public async Task<OrderStatsDto> GetMyOrderStatsAsync(string sellerId)
        {
            return await _unitOfWork.AnalyticsRepository.GetSellerOrderStatsAsync(sellerId);
        }

        public async Task<IReadOnlyList<BestSellingProductDto>> GetMyOutOfStockProductsAsync(string sellerId)
        {
            return await _unitOfWork.AnalyticsRepository.GetSellerOutOfStockProductsAsync(sellerId);
        }

        public async Task<RevenueDto> GetMyRevenueAsync(string sellerId)
        {
            return await _unitOfWork.AnalyticsRepository.GetSellerRevenueAsync(sellerId);
        }
    }
}

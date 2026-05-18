using ECom.Application.DTO.Admin;

namespace ECom.Application.Interfaces.Services
{
    public interface ISellerAnalyticsService
    {
        Task<IReadOnlyList<BestSellingProductDto>> GetMyBestSellingProductsAsync(string sellerId, int count = 10);
        Task<OrderStatsDto> GetMyOrderStatsAsync(string sellerId);
        Task<RevenueDto> GetMyRevenueAsync(string sellerId);
        Task<IReadOnlyList<BestSellingProductDto>> GetMyOutOfStockProductsAsync(string sellerId);
        Task<DashboardSummaryDto> GetMyDashboardSummaryAsync(string sellerId);
    }
}

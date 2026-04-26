using ECom.Application.DTO.Admin;

namespace ECom.Application.Interfaces.Services.Admin
{
    public interface IAdminAnalyticsService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<RevenueDto> GetRevenueAsync();
        Task<OrderStatsDto> GetOrderStatsAsync();
        Task<IReadOnlyList<BestSellingProductDto>> GetBestSellingProductsAsync(int count = 10);
        Task<IReadOnlyList<BestSellingProductDto>> GetOutOfStockProductsAsync();
        Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersAsync(int count = 10);
        Task<IReadOnlyList<CategorySalesDto>> GetCategorySalesAsync();
    }
}

using ECom.Core.DTO.Admin;

namespace ECom.Core.Services.Admin
{
    public interface IAdminAnalyticsService
    {
        // Full dashboard summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();

        // Revenue
        Task<RevenueDto> GetRevenueAsync();

        // Orders
        Task<OrderStatsDto> GetOrderStatsAsync();

        // Products
        Task<IReadOnlyList<BestSellingProductDto>> GetBestSellingProductsAsync(int count = 10);
        Task<IReadOnlyList<BestSellingProductDto>> GetOutOfStockProductsAsync();

        // Customers
        Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersAsync(int count = 10);

        // Categories
        Task<IReadOnlyList<CategorySalesDto>> GetCategorySalesAsync();
    }
}

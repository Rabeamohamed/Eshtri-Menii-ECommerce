using ECom.Application.DTO.Admin;

namespace ECom.Application.Interfaces.Repositories
{
    public interface IAnalyticsRepository
    {
        Task<IReadOnlyList<BestSellingProductDto>> GetBestSellingProductsAsync(int count);
        Task<IReadOnlyList<CategorySalesDto>> GetCategorySalesAsync();
        Task<OrderStatsDto> GetOrderStatsAsync();
        Task<RevenueDto> GetRevenueAsync();
        Task<IReadOnlyList<TopCustomerDto>> GetTopCustomersAsync(int count);
        Task<IReadOnlyList<BestSellingProductDto>> GetOutOfStockProductsAsync();
        Task<int> GetUserCountAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Admin
{
    public record DashboardSummaryDto
    {
        public RevenueDto Revenue { get; init; }
        public OrderStatsDto OrderStats { get; init; }
        public int TotalProducts { get; init; }
        public int OutOfStockProducts { get; init; }
        public int TotalUsers { get; init; }
        public int TotalCategories { get; init; }
        public IReadOnlyList<BestSellingProductDto> BestSellingProducts { get; init; }
        public IReadOnlyList<TopCustomerDto> TopCustomers { get; init; }
        public IReadOnlyList<CategorySalesDto> CategorySales { get; init; }
    }
}

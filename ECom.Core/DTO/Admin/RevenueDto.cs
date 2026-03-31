
namespace ECom.Core.DTO.Admin
{   
    // Revenue
    public record RevenueDto
    {
        public decimal TotalRevenue { get; init; }
        public decimal DailyRevenue { get; init; }
        public decimal WeeklyRevenue { get; init; }
        public decimal MonthlyRevenue { get; init; }
        public decimal AverageOrderValue { get; init; }
    }
}
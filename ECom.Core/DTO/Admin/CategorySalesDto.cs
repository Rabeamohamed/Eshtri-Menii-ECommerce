
namespace ECom.Core.DTO.Admin
{
    public record CategorySalesDto
    {
        public int CategoryId { get; init; }
        public string CategoryName { get; init; }
        public int TotalProductsSold { get; init; }
        public decimal TotalRevenue { get; init; }
    }
}


namespace ECom.Application.DTO.Admin
{
    public record BestSellingProductDto
    {
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public int TotalSold { get; init; }
        public decimal TotalRevenue { get; init; }
        public string CategoryName { get; init; }
    }
}

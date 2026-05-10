using ECom.Core.Enums;

namespace ECom.Application.DTO.Coupon
{
    public record CouponDto
    {
        public int Id { get; init; }
        public string Code { get; init; }
        public string Description { get; init; }
        public string Type { get; init; }
        public decimal DiscountValue { get; init; }
        public decimal MinimumOrderAmount { get; init; }
        public decimal? MaxDiscountAmount { get; init; }
        public DateTime ExpiryDate { get; init; }
        public bool IsActive { get; init; }
        public int UsageLimit { get; init; }
        public int UsageCount { get; init; }
    }
}

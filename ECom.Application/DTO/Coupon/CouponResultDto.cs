namespace ECom.Application.DTO.Coupon
{
    public record CouponResultDto
    {
        public string Code { get; init; }
        public decimal OriginalAmount { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal FinalAmount { get; init; }
        public string Message { get; init; }
        public bool IsSuccess { get; init; }
    }
}

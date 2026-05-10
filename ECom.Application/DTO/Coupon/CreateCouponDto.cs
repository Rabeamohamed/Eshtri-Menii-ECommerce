using ECom.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECom.Application.DTO.Coupon
{
    // Input — create coupon (admin)
    public record CreateCouponDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; init; }

        [MaxLength(500)]
        public string Description { get; init; }

        [Required]
        public string Type { get; init; }  // "Percentage" or "FixedAmount"
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal DiscountValue { get; init; }
        public decimal MinimumOrderAmount { get; init; } = 0;
        public decimal? MaxDiscountAmount { get; init; }
        [Required]
        public DateTime ExpiryDate { get; init; }
        public int UsageLimit { get; init; } = 0;
    }
}
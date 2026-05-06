

using ECom.Core.Enums;

namespace ECom.Core.Entities
{
    public class Coupon:BaseEntity<int>
    {
        public int CouponId { get; set; }
        public string Code { get; set; }           // e.g. "SUMMER20"
        public string Description { get; set; }
        public CouponType Type { get; set; }        // Percentage or FixedAmount
        public decimal DiscountValue { get; set; }  // 20 = 20% or $20
        public decimal MinimumOrderAmount { get; set; } = 0; // min order to apply
        public decimal? MaxDiscountAmount { get; set; }  // cap for percentage discount
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int UsageLimit { get; set; } = 0;   // 0 = unlimited
        public int UsageCount { get; set; } = 0;   // how many times used
        public DateTime CreatedAt { get; set; }
    }
}

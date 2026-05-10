using System.ComponentModel.DataAnnotations;

namespace ECom.Application.DTO.Coupon
{
    // Input — apply coupon
    public record ApplyCouponDto
    {
        [Required]
        public string Code { get; init; }
        [Required]
        public string BasketId { get; init; }
    }
}

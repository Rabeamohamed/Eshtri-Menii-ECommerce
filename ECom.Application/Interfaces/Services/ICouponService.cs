
using ECom.Application.DTO.Coupon;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services
{
    public interface ICouponService
    {
        // Validate and apply coupon
        Task<CouponResultDto> ApplyCouponAsync(ApplyCouponDto dto);

        // Remove coupon from basket
        Task<ResponseAPI> RemoveCouponAsync(string basketId);

        // Validate coupon only (no apply)
        Task<ResponseAPI> ValidateCouponAsync(string code, decimal orderAmount);

        // Admin methods
        Task<IReadOnlyList<CouponDto>> GetAllCouponsAsync();
        Task<CouponDto> GetCouponByCodeAsync(string code);
        Task<ResponseAPI> CreateCouponAsync(CreateCouponDto dto);
        Task<ResponseAPI> UpdateCouponAsync(int id, CreateCouponDto dto);
        Task<ResponseAPI> DeactivateCouponAsync(int couponId);
        Task<ResponseAPI> DeleteCouponAsync(int couponId);
        Task IncrementCouponUsageAsync(string code);
    }
}

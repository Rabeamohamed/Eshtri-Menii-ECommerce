using ECom.Application.DTO.Coupon;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Core.Entities;
using ECom.Core.Enums;

namespace ECom.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CouponResultDto> ApplyCouponAsync(ApplyCouponDto dto)
        {
            var basket = await _unitOfWork.CustomerBasketRepository
                .GetBasketAsync(dto.BasketId);

            if (basket is null)
                return new CouponResultDto { Message = "Basket not found", IsSuccess = false };

            var basketTotal = basket.BasketItems
                .Sum(i => i.Price * i.Quantity);

            var validation = await ValidateCouponAsync(dto.Code, basketTotal);
            if (validation.StatusCode != 200)
                return new CouponResultDto { Message = validation.Message, IsSuccess = false };

            var coupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(dto.Code);

            var discountAmount = CalculateDiscount(coupon!, basketTotal);

            basket.CouponCode = coupon!.Code;
            basket.DiscountAmount = discountAmount;
            var updated = await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
            if (updated is null)
                return new CouponResultDto { Message = "Could not update basket with coupon", IsSuccess = false };

            return new CouponResultDto
            {
                Code = coupon.Code,
                OriginalAmount = basketTotal,
                DiscountAmount = discountAmount,
                FinalAmount = basketTotal - discountAmount,
                Message = $"Coupon applied! You saved EGP {discountAmount:F2}",
                IsSuccess = true
            };
        }

        public async Task<ResponseAPI> RemoveCouponAsync(string basketId)
        {
            var basket = await _unitOfWork.CustomerBasketRepository
                .GetBasketAsync(basketId);

            if (basket is null)
                return new ResponseAPI(404, "Basket not found");

            basket.CouponCode = string.Empty;
            basket.DiscountAmount = 0;

            var updated = await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
            if (updated is null)
                return new ResponseAPI(400, "Could not update basket");

            return new ResponseAPI(200, "Coupon removed successfully");
        }

        public async Task<ResponseAPI> ValidateCouponAsync(string code, decimal orderAmount)
        {
            var coupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(code);

            if (coupon is null)
                return new ResponseAPI(404, "Coupon not found");

            if (!coupon.IsActive)
                return new ResponseAPI(400, "Coupon is no longer active");

            if (coupon.ExpiryDate < DateTime.UtcNow)
                return new ResponseAPI(400, "Coupon has expired");

            if (coupon.UsageLimit > 0 && coupon.UsageCount >= coupon.UsageLimit)
                return new ResponseAPI(400, "Coupon usage limit reached");

            if (orderAmount < coupon.MinimumOrderAmount)
                return new ResponseAPI(400,
                    $"Minimum order amount for this coupon is ${coupon.MinimumOrderAmount:F2}");

            return new ResponseAPI(200, "Coupon is valid");
        }

        public async Task<IReadOnlyList<CouponDto>> GetAllCouponsAsync()
        {
            var coupons = await _unitOfWork.CouponRepository.GetAllAsync();
            return coupons.Select(c => new CouponDto
            {
                Id = c.Id,
                Code = c.Code,
                Description = c.Description,
                Type = c.Type.ToString(),
                DiscountValue = c.DiscountValue,
                MinimumOrderAmount = c.MinimumOrderAmount,
                MaxDiscountAmount = c.MaxDiscountAmount,
                ExpiryDate = c.ExpiryDate,
                IsActive = c.IsActive,
                UsageLimit = c.UsageLimit,
                UsageCount = c.UsageCount
            }).ToList();
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string code)
        {
            var coupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(code);

            if (coupon is null) return null;

            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                Type = coupon.Type.ToString(),
                DiscountValue = coupon.DiscountValue,
                MinimumOrderAmount = coupon.MinimumOrderAmount,
                MaxDiscountAmount = coupon.MaxDiscountAmount,
                ExpiryDate = coupon.ExpiryDate,
                IsActive = coupon.IsActive,
                UsageLimit = coupon.UsageLimit,
                UsageCount = coupon.UsageCount
            };
        }

        public async Task<ResponseAPI> CreateCouponAsync(CreateCouponDto dto)
        {
            var existingCoupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(dto.Code);

            if (existingCoupon != null)
                return new ResponseAPI(400, "Coupon code already exists");

            if (!Enum.TryParse<CouponType>(dto.Type, true, out var couponType))
                return new ResponseAPI(400, "Invalid coupon type. Use 'Percentage' or 'FixedAmount'");

            if (couponType == CouponType.Percentage && dto.DiscountValue > 100)
                return new ResponseAPI(400, "Percentage discount cannot exceed 100%");

            var coupon = new Coupon
            {
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                Type = couponType,
                DiscountValue = dto.DiscountValue,
                MinimumOrderAmount = dto.MinimumOrderAmount,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                ExpiryDate = dto.ExpiryDate,
                UsageLimit = dto.UsageLimit,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CouponRepository.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAPI(201, $"Coupon '{coupon.Code}' created successfully");
        }

        public async Task<ResponseAPI> UpdateCouponAsync(int id, CreateCouponDto dto)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            if (coupon == null)
                return new ResponseAPI(404, "Coupon not found");

            if (coupon.Code.ToUpper() != dto.Code.ToUpper())
            {
                var existingCoupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(dto.Code);
                if (existingCoupon != null)
                    return new ResponseAPI(400, "Coupon code already exists");
            }

            if (!Enum.TryParse<CouponType>(dto.Type, true, out var couponType))
                return new ResponseAPI(400, "Invalid coupon type. Use 'Percentage' or 'FixedAmount'");

            if (couponType == CouponType.Percentage && dto.DiscountValue > 100)
                return new ResponseAPI(400, "Percentage discount cannot exceed 100%");

            coupon.Code = dto.Code.ToUpper();
            coupon.Description = dto.Description;
            coupon.Type = couponType;
            coupon.DiscountValue = dto.DiscountValue;
            coupon.MinimumOrderAmount = dto.MinimumOrderAmount;
            coupon.MaxDiscountAmount = dto.MaxDiscountAmount;
            coupon.ExpiryDate = dto.ExpiryDate;
            coupon.UsageLimit = dto.UsageLimit;

            await _unitOfWork.CouponRepository.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAPI(200, "Coupon updated successfully");
        }

        public async Task<ResponseAPI> DeactivateCouponAsync(int couponId)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(couponId);
            if (coupon is null)
                return new ResponseAPI(404, "Coupon not found");

            coupon.IsActive = false;
            await _unitOfWork.CouponRepository.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAPI(200, "Coupon deactivated successfully");
        }

        public async Task<ResponseAPI> DeleteCouponAsync(int couponId)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(couponId);
            if (coupon is null)
                return new ResponseAPI(404, "Coupon not found");

            await _unitOfWork.CouponRepository.DeleteAsync(couponId);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseAPI(200, "Coupon deleted successfully");
        }

        public async Task IncrementCouponUsageAsync(string code)
        {
            var coupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(code);

            if (coupon != null)
            {
                coupon.UsageCount++;
                await _unitOfWork.CouponRepository.UpdateAsync(coupon);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private static decimal CalculateDiscount(Coupon coupon, decimal orderAmount)
        {
            decimal discount;

            if (coupon.Type == CouponType.Percentage)
            {
                discount = orderAmount * (coupon.DiscountValue / 100);

                if (coupon.MaxDiscountAmount.HasValue)
                    discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);
            }
            else
            {
                discount = coupon.DiscountValue;
                discount = Math.Min(discount, orderAmount);
            }

            return Math.Round(discount, 2);
        }
    }
}

using ECom.Application.DTO.Coupon;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Core.Entities;
using ECom.Core.Enums;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECom.Infrastructure.Service
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
            // 1. Get basket
            var basket = await _unitOfWork.CustomerBasketRepository
                .GetBasketAsync(dto.BasketId);

            if (basket is null)
                return new CouponResultDto { Message = "Basket not found", IsSuccess = false };

            // 2. Calculate current basket total
            var basketTotal = basket.BasketItems
                .Sum(i => i.Price * i.Quantity);

            // 3. Validate coupon
            var validation = await ValidateCouponAsync(dto.Code, basketTotal);
            if (validation.StatusCode != 200)
                return new CouponResultDto { Message = validation.Message, IsSuccess = false };

            // 4. Get coupon
            var coupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(dto.Code);

            // 5. Calculate discount
            var discountAmount = CalculateDiscount(coupon, basketTotal);

            // 6. Apply to basket
            basket.CouponCode = coupon.Code;
            basket.DiscountAmount = discountAmount;
            await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);

            return new CouponResultDto
            {
                Code = coupon.Code,
                OriginalAmount = basketTotal,
                DiscountAmount = discountAmount,
                FinalAmount = basketTotal - discountAmount,
                Message = $"Coupon applied! You saved ${discountAmount:F2}",
                IsSuccess = true
            };
        }

        public async Task<ResponseAPI> RemoveCouponAsync(string basketId)
        {
            var basket = await _unitOfWork.CustomerBasketRepository
                .GetBasketAsync(basketId);

            if (basket is null)
                return new ResponseAPI(404, "Basket not found");

            basket.CouponCode = null;
            basket.DiscountAmount = 0;

            await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
            return new ResponseAPI(200, "Coupon removed successfully");
        }

        public async Task<ResponseAPI> ValidateCouponAsync(string code, decimal orderAmount)
        {
            // 1. Find coupon
            var coupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(code);

            if (coupon is null)
                return new ResponseAPI(404, "Coupon not found");

            // 2. Check if active
            if (!coupon.IsActive)
                return new ResponseAPI(400, "Coupon is no longer active");

            // 3. Check expiry
            if (coupon.ExpiryDate < DateTime.UtcNow)
                return new ResponseAPI(400, "Coupon has expired");

            // 4. Check usage limit
            if (coupon.UsageLimit > 0 && coupon.UsageCount >= coupon.UsageLimit)
                return new ResponseAPI(400, "Coupon usage limit reached");

            // 5. Check minimum order amount
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

        public async Task<CouponDto> GetCouponByCodeAsync(string code)
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
            // Check if code already exists
            var existingCoupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(dto.Code);

            if (existingCoupon != null)
                return new ResponseAPI(400, "Coupon code already exists");

            // Parse type
            if (!Enum.TryParse<CouponType>(dto.Type ,true, out var couponType))
                return new ResponseAPI(400, "Invalid coupon type. Use 'Percentage' or 'FixedAmount'");

            // Validate percentage
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

            // Check if code is being changed and already exists
            if (coupon.Code.ToUpper() != dto.Code.ToUpper())
            {
                var existingCoupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(dto.Code);
                if (existingCoupon != null)
                    return new ResponseAPI(400, "Coupon code already exists");
            }

            // Parse type
            if (!Enum.TryParse<CouponType>(dto.Type, true, out var couponType))
                return new ResponseAPI(400, "Invalid coupon type. Use 'Percentage' or 'FixedAmount'");

            // Validate percentage
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

        // ✅ Private helper — calculate discount amount
        private decimal CalculateDiscount(Coupon coupon, decimal orderAmount)
        {
            decimal discount = 0;

            if (coupon.Type == CouponType.Percentage)
            {
                discount = orderAmount * (coupon.DiscountValue / 100);

                // Apply max discount cap if set
                if (coupon.MaxDiscountAmount.HasValue)
                    discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);
            }
            else // FixedAmount
            {
                discount = coupon.DiscountValue;

                // Can't discount more than order total
                discount = Math.Min(discount, orderAmount);
            }

            return Math.Round(discount, 2);
        }
    }
}

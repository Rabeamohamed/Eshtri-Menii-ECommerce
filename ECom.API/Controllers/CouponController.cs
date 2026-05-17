using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Application.DTO.Coupon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class CouponController : BaseController
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [Authorize]
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto? dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.BasketId) || string.IsNullOrWhiteSpace(dto.Code))
                return BadRequest(new ResponseAPI(400, "Basket id and coupon code are required."));

            var result = await _couponService.ApplyCouponAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new ResponseAPI(400, result.Message));
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("remove/{basketId}")]
        public async Task<IActionResult> RemoveCoupon(string basketId)
        {
            if (string.IsNullOrWhiteSpace(basketId))
                return BadRequest(new ResponseAPI(400, "Basket id is required."));

            var result = await _couponService.RemoveCouponAsync(basketId);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }

        [Authorize]
        [HttpGet("validate/{code}")]
        public async Task<IActionResult> ValidateCoupon(string code, [FromQuery] decimal orderAmount)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(new ResponseAPI(400, "Coupon code is required."));

            var result = await _couponService.ValidateCouponAsync(code, orderAmount);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
    }
}

using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Application.DTO.Coupon;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    public class AdminCouponController : AdminBaseController
    {
        private readonly ICouponService _couponService;

        public AdminCouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return Ok(coupons);
        }

        [HttpGet("get-by-code/{code}")]
        public async Task<IActionResult> GetCouponByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(new ResponseAPI(400, "Coupon code is required."));

            var coupon = await _couponService.GetCouponByCodeAsync(code);
            if (coupon is null)
                return NotFound(new ResponseAPI(404, "Coupon not found"));
            return Ok(coupon);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto? dto)
        {
            if (dto is null)
                return BadRequest(new ResponseAPI(400, "Coupon data is required."));

            var result = await _couponService.CreateCouponAsync(dto);
            return result.StatusCode switch
            {
                201 => StatusCode(201, result),
                _ => BadRequest(result)
            };
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] CreateCouponDto? dto)
        {
            if (dto is null)
                return BadRequest(new ResponseAPI(400, "Coupon data is required."));

            var result = await _couponService.UpdateCouponAsync(id, dto);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateCoupon(int id)
        {
            var result = await _couponService.DeactivateCouponAsync(id);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
    }
}

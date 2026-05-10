using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Application.DTO.Coupon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // POST: api/coupon/apply
        [Authorize]
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto dto)
        {
            try
            {
                var result = await _couponService.ApplyCouponAsync(dto);
                if (!result.IsSuccess)
                    return BadRequest(new ResponseAPI(400, result.Message));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // DELETE: api/coupon/remove/{basketId}
        [Authorize]
        [HttpDelete("remove/{basketId}")]
        public async Task<IActionResult> RemoveCoupon(string basketId)
        {
            try
            {
                var result = await _couponService.RemoveCouponAsync(basketId);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/coupon/validate/{code}
        [Authorize]
        [HttpGet("validate/{code}")]
        public async Task<IActionResult> ValidateCoupon(string code, [FromQuery] decimal orderAmount)
        {
            try
            {
                var result = await _couponService.ValidateCouponAsync(code, orderAmount);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

    }
}

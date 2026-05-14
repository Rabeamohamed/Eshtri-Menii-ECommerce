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
        // GET: api/admin/admincoupon/get-all
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCoupons()
        {
            try
            {
                var coupons = await _couponService.GetAllCouponsAsync();
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/admincoupon/get-by-code/{code}
        [HttpGet("get-by-code/{code}")]
        public async Task<IActionResult> GetCouponByCode(string code)
        {
            try
            {
                var coupon = await _couponService.GetCouponByCodeAsync(code);
                if (coupon is null)
                    return NotFound(new ResponseAPI(404, "Coupon not found"));
                return Ok(coupon);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // POST: api/admin/admincoupon/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
        {
            try
            {
                var result = await _couponService.CreateCouponAsync(dto);
                return result.StatusCode switch
                {
                    201 => Ok(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // PUT: api/admin/admincoupon/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] CreateCouponDto dto)
        {
            try
            {
                var result = await _couponService.UpdateCouponAsync(id, dto);
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

        // PUT: api/admin/admincoupon/deactivate/{id}
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateCoupon(int id)
        {
            try
            {
                var result = await _couponService.DeactivateCouponAsync(id);
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

        // DELETE: api/admin/admincoupon/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            try
            {
                var result = await _couponService.DeleteCouponAsync(id);
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

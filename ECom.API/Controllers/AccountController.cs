using AutoMapper;
using ECom.Application.DTO.Auth;
using ECom.Application.DTO.Order;
using ECom.Core.Entities;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;

namespace ECom.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthService _authService;

        public AccountController(IUnitOfWork work, IMapper mapper, IAuthService authService) 
            : base(work, mapper)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpPut("update-address")]
        public async Task<IActionResult> UpdateAddress(ShippingAddressDto addressDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var address = mapper.Map<Address>(addressDto);
            var result = await _authService.UpdateAddress(email!, address);
            return result ? Ok() : BadRequest();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result != "User Registered Successfully")
            {
                return BadRequest(new ResponseAPI(400, result));
            }
            return Ok(new ResponseAPI(200, result));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                if (result is null)
                    return BadRequest(new ResponseAPI(400, "Email or Password is incorrect"));

                if (result.AccessToken.StartsWith("Please") ||
                    result.AccessToken.StartsWith("Your account"))
                    return BadRequest(new ResponseAPI(400, result.AccessToken));

                SetTokenCookies(result.AccessToken, result.RefreshToken);

                return Ok(new ResponseAPI(200, "Login Successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpPost("active-account")]
        public async Task<IActionResult> ActiveAccount(ActiveEmailDto activeEmailDto)
        {
            var result = await _authService.ActiveEmail(activeEmailDto);
            return result == "User Active Successfully" 
                ? Ok(new ResponseAPI(200, result)) 
                : BadRequest(new ResponseAPI(400, result));
        }

        [HttpGet("send-email-forget-password/{email}")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result = await _authService.SendEmailForForgetPassword(email);
            return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(400));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPassword(resetPasswordDto);
            return result == "Password Reset and changed Successfully" 
                ? Ok(new ResponseAPI(200, result)) 
                : BadRequest(new ResponseAPI(400, result));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new ResponseAPI(401, "No refresh token found"));

                var result = await _authService.RefreshTokenAsync(refreshToken);

                if (result.AccessToken.StartsWith("Invalid") ||
                    result.AccessToken.StartsWith("Refresh token expired"))
                    return Unauthorized(new ResponseAPI(401, result.AccessToken));

                SetTokenCookies(result.AccessToken, result.RefreshToken);

                return Ok(new ResponseAPI(200, "Token refreshed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _authService.RevokeTokenAsync(userId!);

                Response.Cookies.Delete("token");
                Response.Cookies.Delete("refreshToken");

                return Ok(new ResponseAPI(200, "Logged out successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                Domain = "localhost",
                IsEssential = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("token", accessToken, new CookieOptions(cookieOptions)
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions(cookieOptions)
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }
    }
}

using AutoMapper;
using ECom.Application.DTO.Auth;
using ECom.Application.DTO.Order;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.API.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<CookieAuthOptions> _cookieOptions;

        public AccountController(
            IAuthService authService,
            IMapper mapper,
            IOptionsMonitor<CookieAuthOptions> cookieOptions)
        {
            _authService = authService;
            _mapper = mapper;
            _cookieOptions = cookieOptions;
        }

        [Authorize]
        [HttpPut("update-address")]
        public async Task<IActionResult> UpdateAddress([FromBody] ShippingAddressDto? addressDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new ResponseAPI(401, "Not authenticated."));
            if (addressDto is null)
                return BadRequest(new ResponseAPI(400, "Address is required."));

            var address = _mapper.Map<ECom.Core.Entities.Address>(addressDto);
            var result = await _authService.UpdateAddress(email, address);
            return result ? Ok() : BadRequest(new ResponseAPI(400, "Could not update address."));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto? registerDto)
        {
            if (registerDto is null)
                return BadRequest(new ResponseAPI(400, "Registration data is required."));

            var result = await _authService.RegisterAsync(registerDto);
            return Ok(new ResponseAPI(200, result));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto? loginDto)
        {
            if (loginDto is null)
                return BadRequest(new ResponseAPI(400, "Login data is required."));

            var result = await _authService.LoginAsync(loginDto);
            SetTokenCookies(result.AccessToken, result.RefreshToken);

            return Ok(new ResponseAPI(200, "Login Successful"));
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
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new ResponseAPI(401, "No refresh token found"));

            var result = await _authService.RefreshTokenAsync(refreshToken);
            SetTokenCookies(result.AccessToken, result.RefreshToken);

            return Ok(new ResponseAPI(200, "Token refreshed successfully"));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                await _authService.RevokeTokenAsync(userId);

            Response.Cookies.Delete("token");
            Response.Cookies.Delete("refreshToken");

            return Ok(new ResponseAPI(200, "Logged out successfully"));
        }

        [Authorize]
        [HttpGet("get-current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var user = await _authService.GetCurrentUserAsync(email);
            return Ok(user);
        }

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var opt = _cookieOptions.CurrentValue;
            var sameSite = Enum.TryParse<SameSiteMode>(opt.SameSite, true, out var ss)
                ? ss
                : SameSiteMode.Strict;

            var cookieOptions = new CookieOptions
            {
                Secure = opt.Secure,
                HttpOnly = true,
                IsEssential = true,
                SameSite = sameSite
            };

            if (!string.IsNullOrWhiteSpace(opt.Domain))
            {
                cookieOptions.Domain = opt.Domain;
            }

            Response.Cookies.Append("token", accessToken, new CookieOptions(cookieOptions)
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(opt.AccessTokenMinutes)
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions(cookieOptions)
            {
                Expires = DateTimeOffset.UtcNow.AddDays(opt.RefreshTokenDays)
            });
        }
    }
}

using AutoMapper;
using ECom.API.Helper;
using ECom.Core.DTO.Auth;
using ECom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await work.AuthRepository.RegisterAsync(registerDto);
            if (result != "User Registered Successfully")
            {
                return BadRequest(new ResponseAPI(400, result));
            }
            return Ok(new ResponseAPI(200, result));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await work.AuthRepository.LoginAsync(loginDto);

            if (result.StartsWith("Please"))
            {
                return BadRequest(new ResponseAPI(400, result));
            }

            Response.Cookies.Append("token", result, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                Domain = "localhost",
                Expires = DateTimeOffset.UtcNow.AddDays(1),
                IsEssential = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new ResponseAPI(200, result));
        }
        [HttpPost("active-account")]
        public async Task<IActionResult> ActiveAccount(ActiveEmailDto activeEmailDto)
        {
            var result = await work.AuthRepository.ActiveEmail(activeEmailDto);
            return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(400));

        }

        [HttpGet("send-email-forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result = await work.AuthRepository.SendEmailForForgetPassword(email);
            return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(400));

        }
    }
}

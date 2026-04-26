using AutoMapper;
using ECom.Application.DTO.Auth;
using ECom.Application.DTO.Order;
using ECom.Core.Entities;
using ECom.Application.Interfaces;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [Authorize]
        [HttpPut("update-address")]
        public async Task<IActionResult> UpdateAddress(ShippingAddressDto addressDto)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var address = mapper.Map<Address>(addressDto);
            var result = await work.AuthRepository.UpdateAddress(email, address);
            return result ? Ok() : BadRequest();

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
            if (result is null)
                return BadRequest(new ResponseAPI(400, "Email or Password is incorrect"));
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
            return result == "User Active Successfully" ? Ok(new ResponseAPI(200, result)) : BadRequest(new ResponseAPI(400, result));

        }

        [HttpGet("activate-email")]
        public async Task<IActionResult> ActiveEmail(string email, string code)
        {
            var result = await work.AuthRepository.ActiveEmail(new ActiveEmailDto { Email = email, Token = code });
            if (result == "User Active Successfully" || result == "User Already Active")
            {
                return new ContentResult
                {
                    Content = "<html><body><h1>Account Activated Successfully</h1><p>You can now login to your account.</p></body></html>",
                    ContentType = "text/html"
                };
            }
            return new ContentResult
            {
                Content = $"<html><body><h1>Activation Failed</h1><p>{result}</p></body></html>",
                ContentType = "text/html"
            };
        }

        [HttpGet("send-email-forget-password/{email}")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result = await work.AuthRepository.SendEmailForForgetPassword(email);
            return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(400));

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            // Decode the token if it's URL-encoded (comes from email link)
            var decodedDto = new ResetPasswordDto
            {
                Email = resetPasswordDto.Email,
                Password = resetPasswordDto.Password,
                Token = Uri.UnescapeDataString(resetPasswordDto.Token)
            };
            
            var result = await work.AuthRepository.ResetPassword(decodedDto);
            return result == "Password Reset and changed Successfully" ? Ok(new ResponseAPI(200, result)) : BadRequest(new ResponseAPI(400, result));
        }
    }
}

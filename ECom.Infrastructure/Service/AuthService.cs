using ECom.Application.DTO.Auth;
using ECom.Application.Interfaces.Services;
using ECom.Core.Entities;
using ECom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ECom.Infrastructure.Repositories;

namespace ECom.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IGenerateToken _generateToken;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<AppUser> userManager,
            IEmailService emailService,
            SignInManager<AppUser> signInManager,
            IGenerateToken generateToken,
            AppDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _generateToken = generateToken;
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto == null) return null;

            if (await _userManager.FindByNameAsync(registerDto.UserName) is not null)
                return "This Username already Registered";

            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
                return "This Email already Registered";

            AppUser user = new()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return result.Errors.First().Description;

            await _userManager.AddToRoleAsync(user, "Customer");

            // Send Activation Email
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendActivationEmail(user.Email!, token, "Active", "Active Email", "Please Active your Email, Click on button to Active");

            return "User Registered Successfully";
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null) return null;

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
                return new AuthResponseDto { AccessToken = "Email or Password is incorrect" };

            if (user.IsBlocked)
                return new AuthResponseDto
                {
                    AccessToken = $"Your account has been blocked. Reason: {user.BlockReason ?? "Violation of terms"}"
                };

            if (!user.EmailConfirmed)
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await SendActivationEmail(user.Email!, token, "Active", "Active Email", "Please Active your Email, Click on button to Active");
                return new AuthResponseDto
                {
                    AccessToken = "Please Active your Email, We have sent you an email to active your account"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (!result.Succeeded)
                return new AuthResponseDto { AccessToken = "Please Check your E-mail or Password, Something went wrong" };

            var accessToken = await _generateToken.GetAndGenerateToken(user);
            var refreshToken = _generateToken.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = (DateTime)user.RefreshTokenExpiryTime
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user is null)
                return new AuthResponseDto { AccessToken = "Invalid refresh token" };

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return new AuthResponseDto { AccessToken = "Refresh token expired — please login again" };

            var newAccessToken = await _generateToken.GetAndGenerateToken(user);
            var newRefreshToken = _generateToken.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = (DateTime)user.RefreshTokenExpiryTime
            };
        }

        public async Task<bool> RevokeTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<bool> SendEmailForForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await SendActivationEmail(user.Email!, token, "Reset-Password", "Reset Password", "Please Click on button to Reset your Password");
            return true;
        }

        public async Task<string> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user is null) return "Invalid Email";

            // Decode the token if it's URL-encoded (comes from email link)
            var decodedToken = Uri.UnescapeDataString(resetPasswordDto.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.Password);
            if (result.Succeeded)
                return "Password Reset and changed Successfully";

            return result.Errors.First().Description;
        }

        public async Task<string> ActiveEmail(ActiveEmailDto activeEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(activeEmailDto.Email);
            if (user is null) return "User not found";

            if (await _userManager.IsEmailConfirmedAsync(user))
                return "User Already Active";

            var result = await _userManager.ConfirmEmailAsync(user, activeEmailDto.Token);
            if (result.Succeeded)
                return "User Active Successfully";

            return result.Errors.FirstOrDefault()?.Description ?? "Failed to activate user";
        }

        public async Task<bool> UpdateAddress(string email, Address address)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var findUser = await _userManager.FindByEmailAsync(email);
            if (findUser is null) return false;

            var myAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.AppUserId == findUser.Id);
            if (myAddress is null)
            {
                address.AppUserId = findUser.Id;
                await _context.Addresses.AddAsync(address);
            }
            else
            {
                address.Id = myAddress.Id;
                address.AppUserId = findUser.Id;
                _context.Entry(myAddress).State = EntityState.Detached;
                _context.Addresses.Update(address);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task SendActivationEmail(string email, string code, string component, string subject, string message)
        {
            var baseUrl = _configuration["Token:Issuer"] ?? "https://localhost:44358";
            var result = new EmailDto(email,
                "rabea2mohamed@gmail.com",
                subject,
                EmailStringBody.send(email, code, component, message, baseUrl));
            await _emailService.SendEmailAsync(result);
        }
    }
}

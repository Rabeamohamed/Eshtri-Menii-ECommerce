using ECom.Application.DTO.Auth;
using ECom.Application.Email;
using ECom.Application.Interfaces.Persistence;
using ECom.Application.Interfaces.Services;
using ECom.Core.Entities;
using ECom.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECom.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IGenerateToken _generateToken;
        private readonly IUserAddressPersistence _userAddressPersistence;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<AppUser> userManager,
            IEmailService emailService,
            SignInManager<AppUser> signInManager,
            IGenerateToken generateToken,
            IUserAddressPersistence userAddressPersistence,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _generateToken = generateToken;
            _userAddressPersistence = userAddressPersistence;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto == null) return null!;

            if (await _userManager.FindByNameAsync(registerDto.UserName) is not null)
                throw new BusinessException("This Username already Registered", 400);

            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
                throw new BusinessException("This Email already Registered", 400);

            AppUser user = new()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                throw new BusinessException(result.Errors.First().Description, 400);

            // Ensure valid role selection (Prevent users from registering as Admin)
            var roleToAssign = string.Equals(registerDto.Role, "Vendor", StringComparison.OrdinalIgnoreCase) 
                ? "Vendor" 
                : "Customer";

            await _userManager.AddToRoleAsync(user, roleToAssign);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendActivationEmail(user.Email!, token, "Active", "Active Email", "Please Active your Email, Click on button to Active");

            return "User Registered Successfully";
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null) return null!;

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
                throw new BusinessException("Email or Password is incorrect", 401);

            if (user.IsBlocked)
                throw new BusinessException($"Your account has been blocked. Reason: {user.BlockReason ?? "Violation of terms"}", 403);

            if (!user.EmailConfirmed)
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await SendActivationEmail(user.Email!, token, "Active", "Active Email", "Please Active your Email, Click on button to Active");
                throw new BusinessException("Please Active your Email, We have sent you an email to active your account", 400);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (!result.Succeeded)
                throw new BusinessException("Email or Password is incorrect", 401);

            var accessToken = await _generateToken.GetAndGenerateToken(user);
            var refreshToken = _generateToken.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = (DateTime)user.RefreshTokenExpiryTime!
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user is null)
                throw new BusinessException("Invalid refresh token", 401);

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new BusinessException("Refresh token expired — please login again", 401);

            var newAccessToken = await _generateToken.GetAndGenerateToken(user);
            var newRefreshToken = _generateToken.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = (DateTime)user.RefreshTokenExpiryTime!
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

            return await _userAddressPersistence.UpsertUserAddressAsync(findUser.Id, address);
        }

        public async Task<UserAuthDto?> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserAuthDto
            {
                Email = user.Email!,
                DisplayName = user.DisplayName,
                UserName = user.UserName!,
                Roles = roles.ToList()
            };
        }

        private async Task SendActivationEmail(string email, string code, string component, string subject, string message)
        {
            var baseUrl = _configuration["Token:Issuer"] ?? "https://localhost:44358";
            var dto = new EmailDto(email,
                "rabea2mohamed@gmail.com",
                subject,
                EmailStringBody.Send(email, code, component, message, baseUrl));
            await _emailService.SendEmailAsync(dto);
        }
    }
}

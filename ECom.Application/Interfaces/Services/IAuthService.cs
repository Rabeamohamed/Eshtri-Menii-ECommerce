using ECom.Application.DTO.Auth;
using ECom.Core.Entities;

namespace ECom.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string userId);
        Task<bool> SendEmailForForgetPassword(string email);
        Task<string> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<string> ActiveEmail(ActiveEmailDto activeEmailDto);
        Task<bool> UpdateAddress(string email, Address address);
        Task<UserAuthDto?> GetCurrentUserAsync(string email);
    }
}

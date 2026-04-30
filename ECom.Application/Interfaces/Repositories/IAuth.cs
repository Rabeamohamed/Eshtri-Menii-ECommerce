using ECom.Application.DTO.Auth;
using ECom.Core.Entities;

namespace ECom.Application.Interfaces.Repositories
{
    public interface IAuth
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);   // returns AuthResponseDto
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken); //  new
        Task<bool> RevokeTokenAsync(string userId);            //  new — logout
        Task SendEmail(string email, string code, string component, string subject, string message);
        Task<bool> SendEmailForForgetPassword(string email);
        Task<string> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<string> ActiveEmail(ActiveEmailDto activeEmailDto);
        Task<bool> UpdateAddress(string email, Address address);

    }
}




using ECom.Core.DTO.Auth;
using ECom.Core.Entities;

namespace ECom.Core.Interfaces
{
    public interface IAuth
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task SendEmail(string email, string code, string component, string subject, string message);
        Task<bool> SendEmailForForgetPassword(string email);
        Task<string> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<string> ActiveEmail(ActiveEmailDto activeEmailDto);
        Task<bool> UpdateAddress(string email , Address address);
      
    }
}

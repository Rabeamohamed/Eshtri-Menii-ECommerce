

using ECom.Core.DTO.Auth;

namespace ECom.Core.Interfaces
{
    public interface IAuth
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task SendEmail(string email, string code, string component, string subject, string message);
        Task<bool> SendEmailForForgetPassword(string email);
        public Task<string> ResetPassword(ResetPasswordDto resetPasswordDto);
        public Task<string> ActiveEmail(ActiveEmailDto activeEmailDto);
    }
}

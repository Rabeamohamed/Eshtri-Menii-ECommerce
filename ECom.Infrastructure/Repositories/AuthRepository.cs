using ECom.Core.DTO;
using ECom.Core.DTO.Auth;
using ECom.Core.Interfaces;
using ECom.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace ECom.Infrastructure.Repositories
{
    public class AuthRepository : IAuth
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IGenerateToken _generateToken;
        public AuthRepository(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken generateToken)
        {
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _generateToken = generateToken;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {

            if (registerDto == null)
            {
                return null;
            }
            if (await _userManager.FindByNameAsync(registerDto.UserName) is not null)
            {
                return "This Username already Registered";
            }

            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
            {
                return "This Email already Registered";
            }

            AppUser user = new()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded is not true)
            {
                return result.Errors.ToList()[0].Description;
            }

            // Send Actice or Confirmation Email
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendEmail(user.Email, token, "Active", "Active Email", "Please Active your Email, Click on button to Active");

            return "User Registered Successfully";

        }

        public async Task SendEmail(string email, string code, string component, string subject, string message)
        {
            var result = new EmailDto(email,
                "rabea2mohamed@gmail.com",
                subject,
                EmailStringBody.send(email, code, component, message));
            await _emailService.SendEmailAsync(result);
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return null;
            }
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (!user.EmailConfirmed)
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await SendEmail(user.Email, token, "Active", "Active Email", "Please Active your Email, Click on button to Active");
                return "Please Active your Email, We have sent you an email to active your account";
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);

            if (result.Succeeded)
            {
                return _generateToken.GetAndGenerateToken(user);
            }

            return "UPlease Check your E-mail or Password , Something went wrong";
        }

        public async Task<bool> SendEmailForForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return false;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await SendEmail(user.Email, token, "Reset-Password", "Reset Password", "Please Click on button to Reset your Password");
            return true;
        }

        public async Task<string> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user is null)
            {
                return "Invalid Email";
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            if (result.Succeeded)
            {
                return "Password Reset and changed Successfully";
            }
            return result.Errors.ToList()[0].Description;
        }

        public async Task<bool> ActiveEmail(ActiveEmailDto activeEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(activeEmailDto.Email);
            if (user is null)
            {
                return false;
            }
            var result = await _userManager.ConfirmEmailAsync(user, activeEmailDto.Token);
            if (result.Succeeded)
            {
                return result.Succeeded;
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendEmail(user.Email, token, "Active", "Active Email", "Please Active your Email, Click on button to Active");
            return false;
        }
    }
} 

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
        public AuthRepository(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
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
                return "Login done Successfully";
            }
          
            return "UPlease Check your E-mail or Password , Something went wrong";
        }
    }
}

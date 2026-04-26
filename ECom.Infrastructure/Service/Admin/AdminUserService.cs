using ECom.Application.DTO.Admin.User;
using ECom.Application.DTO.Auth;
using ECom.Core.Entities;
using ECom.Application.Services;
using ECom.Application.Services.Admin;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace ECom.Infrastructure.Service.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        public AdminUserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }
        public async Task<ResponseAPI> AssignRoleAsync(AssignRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user is null)
                return new ResponseAPI(404, "User not found");

            // Check role exists
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return new ResponseAPI(404, $"Role '{dto.Role}' not found");

            // Check if already has role
            if (await _userManager.IsInRoleAsync(user, dto.Role))
                return new ResponseAPI(400, $"User already has role '{dto.Role}'");

            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded)
                return new ResponseAPI(400, result.Errors.First().Description);

            return new ResponseAPI(200, $"Role '{dto.Role}' assigned successfully");
        }

        public async Task<ResponseAPI> BlockUserAsync(BlockUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user is null)
                return new ResponseAPI(404, "User not found");

            if (user.IsBlocked)
                return new ResponseAPI(400, "User is already blocked");

            // Block user
            user.IsBlocked = true;
            user.BlockReason = dto.Reason;
            user.BlockedAt = DateTime.UtcNow;

            // Lock user out of the system
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            await _userManager.UpdateAsync(user);
            // Send block notification email
            await SendBlockEmail(user.Email, user.UserName, dto.Reason, isBlocked: true);
            return new ResponseAPI(200, "User blocked successfully");
        }

        public async Task<IReadOnlyList<string>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return roles;
        }

        public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    IsBlocked = user.IsBlocked,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles
                });
            }

            return result;
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName,
                IsBlocked = user.IsBlocked,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles
            };
        }

        public async Task<ResponseAPI> RemoveRoleAsync(AssignRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user is null)
                return new ResponseAPI(404, "User not found");

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return new ResponseAPI(404, $"Role '{dto.Role}' not found");

            if (!await _userManager.IsInRoleAsync(user, dto.Role))
                return new ResponseAPI(400, $"User does not have role '{dto.Role}'");

            var result = await _userManager.RemoveFromRoleAsync(user, dto.Role);
            if (!result.Succeeded)
                return new ResponseAPI(400, result.Errors.First().Description);

            return new ResponseAPI(200, $"Role '{dto.Role}' removed successfully");
        }

        public async Task<ResponseAPI> UnblockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return new ResponseAPI(404, "User not found");

            if (!user.IsBlocked)
                return new ResponseAPI(400, "User is not blocked");

            user.IsBlocked = false;
            user.BlockReason = null;
            user.BlockedAt = null;

            // Unlock user
            await _userManager.SetLockoutEndDateAsync(user, null);

            await _userManager.UpdateAsync(user);
            
            // Send unblock notification email
            await SendBlockEmail(user.Email, user.UserName, string.Empty, isBlocked: false);
            return new ResponseAPI(200, "User unblocked successfully");
        }

        private async Task SendBlockEmail(string email, string userName, string reason, bool isBlocked)
        {
            string subject = isBlocked ? "Your account has been blocked" : "Your account has been unblocked";
            string content = isBlocked
                ? $"Dear {userName},<br/><br/>Your account has been blocked. Reason: {reason}"
                : $"Dear {userName},<br/><br/>Your account has been unblocked. You can now login.";

            var emailDto = new EmailDto(email, "admin@eshtry.com", subject, content);
            await _emailService.SendEmailAsync(emailDto);
        }
    }
}

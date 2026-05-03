using ECom.Application.DTO.Admin.User;
using ECom.Application.DTO.Auth;
using ECom.Application.Interfaces.Services;
using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Sharing;
using ECom.Core.Entities;
using Hangfire;
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
            //  Fire and forget — don't block HTTP request waiting for email
            //BackgroundJob.Enqueue<IBackgroundJobService>(
            //    job => job.SendBlockNotificationAsync(user.Id, true, dto.Reason));

            // Send email directly
            await SendBlockNotificationEmail(
                user.Email,
                user.UserName,
                dto.Reason,
                isBlocked: true);
            return new ResponseAPI(200, "User blocked successfully");
        }

        public async Task<IReadOnlyList<string>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return roles;
        }

        public async Task<IReadOnlyList<UserAdminDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserAdminDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserAdminDto
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

        public async Task<UserAdminDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserAdminDto
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
            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.UpdateAsync(user);

            // Send unblock notification email
            //  Fire and forget
            //BackgroundJob.Enqueue<IBackgroundJobService>(
            //    job => job.SendBlockNotificationAsync(user.Id, false, null));

            //  Send email directly
            await SendBlockNotificationEmail(
                user.Email,
                user.UserName,
                null,
                isBlocked: false);

            return new ResponseAPI(200, "User unblocked successfully");
        }

        //  Private helper — send block/unblock email
        private async Task SendBlockNotificationEmail(
            string email,
            string userName,
            string reason,
            bool isBlocked)
        {
            var subject = isBlocked
                ? "⛔ Account Blocked"
                : "✅ Account Unblocked";

            var message = isBlocked
                ? $"Dear {userName},\n\n" +
                  $"Your account has been blocked.\n" +
                  $"Reason: {reason ?? "Violation of terms"}\n\n" +
                  $"Please contact support if you think this is a mistake."
                : $"Dear {userName},\n\n" +
                  $"Your account has been unblocked.\n" +
                  $"You can now login again.\n\n" +
                  $"Thank you for your patience.";

            var emailDto = new EmailDto(
                email,
                "noreply@ecom.com",
                subject,
                message
            );

            await _emailService.SendEmailAsync(emailDto);
        }
    }
}

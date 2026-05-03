using ECom.Application.DTO.Admin.User;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services.Admin
{
    public interface IAdminUserService
    {
        // Get all users
        Task<IReadOnlyList<UserAdminDto>> GetAllUsersAsync();

        // Get user by id
        Task<UserAdminDto> GetUserByIdAsync(string userId);

        // Block user
        Task<ResponseAPI> BlockUserAsync(BlockUserDto dto);

        // Unblock user
        Task<ResponseAPI> UnblockUserAsync(string userId);

        // Assign role to user
        Task<ResponseAPI> AssignRoleAsync(AssignRoleDto dto);

        // Remove role from user
        Task<ResponseAPI> RemoveRoleAsync(AssignRoleDto dto);

        // Get all roles
        Task<IReadOnlyList<string>> GetAllRolesAsync();
    }
}

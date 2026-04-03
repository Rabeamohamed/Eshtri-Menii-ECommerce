using ECom.Core.DTO.Admin.User;
using ECom.Core.Services.Admin;
using ECom.Core.Sharing;
namespace ECom.Infrastructure.Service.Admin
{
    public class AdminUserService : IAdminUserService
    {
        public Task<ResponseAPI> AssignRoleAsync(AssignRoleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseAPI> BlockUserAsync(BlockUserDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<string>> GetAllRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<UserDto>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseAPI> RemoveRoleAsync(AssignRoleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseAPI> UnblockUserAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}

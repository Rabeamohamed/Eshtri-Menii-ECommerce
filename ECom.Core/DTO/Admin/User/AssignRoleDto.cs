
namespace ECom.Core.DTO.Admin.User
{
    public record AssignRoleDto
    {
        public string UserId { get; init; }
        public string Role { get; init; }
    }
}

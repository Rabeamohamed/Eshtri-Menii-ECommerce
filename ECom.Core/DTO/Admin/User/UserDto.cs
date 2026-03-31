
namespace ECom.Core.DTO.Admin.User
{
    public record UserDto
    {
        public string Id { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public string DisplayName { get; init; }
        public bool IsBlocked { get; init; }
        public bool EmailConfirmed { get; init; }
        public IList<string> Roles { get; init; }
    }
}

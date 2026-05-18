

namespace ECom.Application.DTO.Auth
{
    public record RegisterDto: LoginDto
    {
        public string UserName { get; init; }
        public string DisplayName { get; init; }
        public string Role { get; init; } = "Customer";
    }
}

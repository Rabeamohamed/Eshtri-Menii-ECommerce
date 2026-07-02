

namespace ECom.Application.DTO.Auth
{
    public record ResetPasswordDto : LoginDto
    {
        public string Token { get; init; }
    }
}

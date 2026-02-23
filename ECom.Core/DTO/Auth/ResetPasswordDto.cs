

namespace ECom.Core.DTO.Auth
{
    public record ResetPasswordDto :LoginDto
    {
        public string Token { get; set; }
    }
}

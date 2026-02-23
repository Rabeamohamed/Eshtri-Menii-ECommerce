

namespace ECom.Core.DTO.Auth
{
    public record ActiveEmailDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}

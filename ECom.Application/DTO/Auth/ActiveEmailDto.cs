

namespace ECom.Application.DTO.Auth
{
    public record ActiveEmailDto
    {
        public string Email { get; init; }
        public string Token { get; init; }
    }
}

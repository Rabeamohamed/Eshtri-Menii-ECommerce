
namespace ECom.Application.DTO.Auth
{
    public record AuthResponseDto
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public DateTime RefreshTokenExpiry { get; init; }
    }
}

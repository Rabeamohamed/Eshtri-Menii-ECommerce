

namespace ECom.Core.DTO.Auth
{
    public record RegisterDto: LoginDto
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
    }
}

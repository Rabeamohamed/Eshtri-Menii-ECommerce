namespace ECom.Application.DTO.Auth
{
    public class UserAuthDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}

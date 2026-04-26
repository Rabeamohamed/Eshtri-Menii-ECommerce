namespace ECom.Application.DTO.Admin.User
{
    public record BlockUserDto
    {
        public string UserId { get; init; }
        public string Reason { get; init; }
    }
}

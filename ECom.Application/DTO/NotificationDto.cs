using ECom.Core.Enums;

namespace ECom.Application.DTO
{
    public record NotificationDto
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Message { get; init; }
        public NotificationType Type { get; init; }
        public bool IsRead { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}



using ECom.Core.Enums;

namespace ECom.Core.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public string? UserId { get; set; }
        public bool IsAdminOnly { get; set; } = false;
        public virtual AppUser? User { get; set; }
    }
}

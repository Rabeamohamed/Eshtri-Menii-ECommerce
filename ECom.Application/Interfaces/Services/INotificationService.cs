
using ECom.Application.DTO;
using ECom.Core.Enums;

namespace ECom.Application.Interfaces.Services
{
    public interface INotificationService
    {
        // Get user notifications
        Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(string userId);

        // Get unread count
        Task<int> GetUnreadCountAsync(string userId);

        // Mark as read
        Task MarkAsReadAsync(int notificationId, string userId);

        // Mark all as read
        Task MarkAllAsReadAsync(string userId);

        // Send to specific user
        Task SendToUserAsync(string userId, string title, string message, NotificationType type);

        // Send to all users
        Task BroadcastAsync(string title, string message, NotificationType type);

        // Send to admins only
        Task SendToAdminsAsync(string title, string message, NotificationType type);
    }
}

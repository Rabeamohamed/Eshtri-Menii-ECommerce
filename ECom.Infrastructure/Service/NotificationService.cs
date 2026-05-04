
using ECom.Application.DTO;
using ECom.Application.Interfaces.Services;
using ECom.Core.Entities;
using ECom.Core.Enums;
using ECom.Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ECom.Infrastructure.Hubs;

namespace ECom.Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            AppDbContext context,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(
            string userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => (n.UserId == userId || n.UserId == null) && !n.IsAdminOnly)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n =>
                    (n.UserId == userId || n.UserId == null) &&
                    !n.IsAdminOnly &&
                    !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == notificationId &&
                    n.UserId == userId);

            if (notification is null) return;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
                notification.IsRead = true;

            await _context.SaveChangesAsync();
        }

        public async Task SendToUserAsync(
            string userId, string title, string message, NotificationType type)
        {
            // 1. Save to DB
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                UserId = userId,
                IsAdminOnly = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // 2. Push real-time to specific user group
            await _hubContext.Clients.Group(userId)
                .SendAsync("ReceiveNotification", new NotificationDto
                {
                    Id = notification.Id,
                    Title = title,
                    Message = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt
                });
        }

        public async Task BroadcastAsync(
            string title, string message, NotificationType type)
        {
            // 1. Save to DB — null UserId = broadcast
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                UserId = null,  // ✅ null = all users
                IsAdminOnly = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // 2. Push to ALL connected clients
            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", new NotificationDto
                {
                    Id = notification.Id,
                    Title = title,
                    Message = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt
                });
        }

        public async Task SendToAdminsAsync(
            string title, string message, NotificationType type)
        {
            // 1. Save to DB
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                UserId = null,
                IsAdminOnly = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // 2. Push to Admins group only
            await _hubContext.Clients.Group("Admins")
                .SendAsync("ReceiveNotification", new NotificationDto
                {
                    Id = notification.Id,
                    Title = title,
                    Message = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = notification.CreatedAt
                });
        }
    }
}

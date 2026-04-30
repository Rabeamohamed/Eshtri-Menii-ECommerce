namespace ECom.Application.Interfaces.Services
{
    public interface IBackgroundJobService
    {
        // Recurring jobs
        Task SendDailySalesReportAsync();
        Task CleanupCancelledOrdersAsync();
        Task SendLowStockAlertsAsync();

        // Fire and forget
        Task SendBlockNotificationAsync(string userId, bool isBlocked, string reason);
        Task SendOrderConfirmationAsync(int orderId);
    }
}

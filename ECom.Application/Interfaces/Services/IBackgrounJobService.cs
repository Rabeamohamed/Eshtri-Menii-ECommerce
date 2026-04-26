using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

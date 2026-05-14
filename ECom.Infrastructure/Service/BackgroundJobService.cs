using ECom.Application.DTO.Auth;
using ECom.Application.Interfaces.Services;
using ECom.Core.Entities;
using ECom.Core.Entities.Order;
using ECom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECom.Infrastructure.Service
{
    internal class BackgroundJobService: IBackgroundJobService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public BackgroundJobService(
            AppDbContext context,
            IEmailService emailService,
            UserManager<AppUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
        }

        //  Recurring — runs daily
        public async Task SendDailySalesReportAsync()
        {
            var today = DateTime.UtcNow.Date;

            // Get today's completed orders
            var todayOrders = await _context.Orders
                .Where(o => o.Status == PaymentStatus.PaymentReceived &&
                       o.OrderDate >= today)
                .ToListAsync();

            var totalRevenue = todayOrders.Sum(o => o.SubTotal);
            var totalOrders = todayOrders.Count;

            // Get admin email from config
            var adminEmail = _configuration["AdminSettings:Email"] ?? "admin@ecom.com";

            var message = $@"
                Daily Sales Report - {today:dd/MM/yyyy}
                ================================
                Total Orders: {totalOrders}
                Total Revenue: ${totalRevenue:F2}
                Average Order Value: ${(totalOrders > 0 ? totalRevenue / totalOrders : 0):F2}
            ";

            var emailDto = new EmailDto(
                adminEmail,
                "noreply@ecom.com",
                $"Daily Sales Report - {today:dd/MM/yyyy}",
                message
            );

            await _emailService.SendEmailAsync(emailDto);
        }

        //  Recurring — runs daily
        public async Task CleanupCancelledOrdersAsync()
        {
            // Delete cancelled orders older than 30 days
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            var oldCancelledOrders = await _context.Orders
                .Where(o => o.Status == PaymentStatus.Cancelled &&
                       o.OrderDate < cutoffDate)
                .ToListAsync();

            if (oldCancelledOrders.Any())
            {
                _context.Orders.RemoveRange(oldCancelledOrders);
                await _context.SaveChangesAsync();
            }
        }

        //  Recurring — runs daily
        public async Task SendLowStockAlertsAsync()
        {
            // Products with stock less than 5
            var lowStockProducts = await _context.Products
                .Where(p => p.StockQuantity > 0 && p.StockQuantity <= 5)
                .Include(p => p.Category)
                .ToListAsync();

            if (!lowStockProducts.Any()) return;

            var adminEmail = _configuration["AdminSettings:Email"] ?? "admin@ecom.com";

            var productList = string.Join("\n", lowStockProducts.Select(p =>
                $"- {p.Name} (Category: {p.Category.Name}) — Stock: {p.StockQuantity}"));

            var message = $@"
                Low Stock Alert - {DateTime.UtcNow:dd/MM/yyyy}
                ================================
                The following products are running low on stock:
                {productList}
                
                Please restock these items soon.
            ";

            var emailDto = new EmailDto(
                adminEmail,
                "noreply@ecom.com",
                "⚠️ Low Stock Alert",
                message
            );

            await _emailService.SendEmailAsync(emailDto);
        }

        //  Fire and forget — instant background
        public async Task SendBlockNotificationAsync(
            string userId, bool isBlocked, string reason)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return;

            var subject = isBlocked ? "Account Blocked" : "Account Unblocked";
            var message = isBlocked
                ? $"Your account has been blocked.\nReason: {reason ?? "Violation of terms"}\nPlease contact support."
                : "Your account has been unblocked.\nYou can now login again.";

            var emailDto = new EmailDto(
                user.Email,
                "noreply@ecom.com",
                subject,
                message
            );

            await _emailService.SendEmailAsync(emailDto);
        }

        //  Fire and forget — instant background
        public async Task SendOrderConfirmationAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethod)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null) return;

            var itemsList = string.Join("\n", order.OrderItems.Select(i =>
                $"- {i.ProductName} x{i.Quantity} = ${i.Price * i.Quantity:F2}"));

            var message = $@"
                Order Confirmation - #{order.Id}
                ================================
                Thank you for your order!
                
                Order Date: {order.OrderDate:dd/MM/yyyy}
                Delivery Method: {order.DeliveryMethod.Name}
                
                Items:
                {itemsList}
                
                Subtotal: ${order.SubTotal:F2}
                Total: ${order.GetTotal():F2}
                
                Status: {order.Status}
            ";

            var emailDto = new EmailDto(
                order.BuyerEmail,
                "noreply@ecom.com",
                $"Order Confirmation #{order.Id}",
                message
            );

            await _emailService.SendEmailAsync(emailDto);
        }
    }

    }


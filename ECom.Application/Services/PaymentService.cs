using ECom.Application.Interfaces.Payments;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using ECom.Core.Entities;
using ECom.Core.Entities.Order;
using ECom.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ECom.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStripePaymentGateway _stripePaymentGateway;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            UserManager<AppUser> userManager,
            IStripePaymentGateway stripePaymentGateway,
            ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _userManager = userManager;
            _stripePaymentGateway = stripePaymentGateway;
            _logger = logger;
        }

        public async Task<CustomerBasket?> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethodId)
        {
            var basket = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(basketId);
            if (basket == null) return null;

            decimal shippingPrice = 0m;
            if (deliveryMethodId.HasValue)
            {
                var delivery = await _unitOfWork.DeliveryMethodRepository.GetByIdAsync(deliveryMethodId.Value);
                if (delivery != null)
                {
                    shippingPrice = delivery.Price;
                }
            }

            foreach (var item in basket.BasketItems)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
                if (product != null)
                {
                    item.Price = product.NewPrice;
                }
            }

            var subTotal = basket.BasketItems.Sum(x => x.Quantity * x.Price);
            var discountAmount = basket.DiscountAmount > 0 ? basket.DiscountAmount : 0m;
            var finalTotal = subTotal - discountAmount + shippingPrice;
            var amountCents = (long)(finalTotal * 100);

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var (id, secret) = await _stripePaymentGateway.CreatePaymentIntentAsync(
                    amountCents,
                    "usd",
                    new[] { "card" });
                basket.PaymentIntentId = id;
                basket.ClientSecret = secret;
            }
            else
            {
                await _stripePaymentGateway.UpdatePaymentIntentAmountAsync(basket.PaymentIntentId, amountCents);
            }

            await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public Task<bool> RefundPaymentAsync(string paymentIntentId) =>
            _stripePaymentGateway.RefundPaymentIntentAsync(paymentIntentId);

        public async Task UpdateOrderPaymentStatusAsync(string paymentIntentId, PaymentStatus status)
        {
            var order = await _unitOfWork.OrderRepository
                .GetOrderByPaymentIntentIdAsync(paymentIntentId);

            if (order is null) return;

            if (order.Status == status)
                return;

            order.Status = status;
            await _unitOfWork.OrderRepository.UpdateOrderAsync(order);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                var user = await _userManager.FindByEmailAsync(order.BuyerEmail);
                if (user != null)
                {
                    var (title, message) = status switch
                    {
                        PaymentStatus.PaymentReceived => ("Payment Received! ✅", $"Your payment for order #{order.Id} has been received successfully."),
                        PaymentStatus.PaymentFailed => ("Payment Failed! ❌", $"Payment for your order #{order.Id} failed. Please check your payment method."),
                        _ => (string.Empty, string.Empty)
                    };

                    if (!string.IsNullOrEmpty(title))
                    {
                        await _notificationService.SendToUserAsync(
                            user.Id,
                            title,
                            message,
                            status == PaymentStatus.PaymentReceived ? NotificationType.PaymentReceived : NotificationType.PaymentFailed
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Notification failed for order {OrderId} payment status {Status}", order.Id, status);
            }
        }
    }
}

using ECom.Core.Entities;
using ECom.Core.Entities.Order;


namespace ECom.Core.Services
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethodId);
        Task<bool> RefundPaymentAsync(string paymentIntentId);
        Task UpdateOrderPaymentStatusAsync(string paymentIntentId, PaymentStatus status);
    }
}

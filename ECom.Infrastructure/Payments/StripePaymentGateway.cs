using ECom.Application.Interfaces.Payments;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace ECom.Infrastructure.Payments
{
    public class StripePaymentGateway : IStripePaymentGateway
    {
        public StripePaymentGateway(IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = configuration["StripSettings:SecretKey"]
                ?? throw new InvalidOperationException("StripSettings:SecretKey is not configured.");
        }

        public async Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
            long amountCents,
            string currency,
            IReadOnlyList<string> paymentMethodTypes,
            CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = amountCents,
                Currency = currency,
                PaymentMethodTypes = paymentMethodTypes.ToList()
            };
            var intent = await service.CreateAsync(options, cancellationToken: cancellationToken);
            return (intent.Id, intent.ClientSecret);
        }

        public async Task UpdatePaymentIntentAmountAsync(
            string paymentIntentId,
            long amountCents,
            CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentUpdateOptions { Amount = amountCents };
            await service.UpdateAsync(paymentIntentId, options, cancellationToken: cancellationToken);
        }

        public async Task<bool> RefundPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            var refundService = new RefundService();
            var refund = await refundService.CreateAsync(new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Reason = RefundReasons.RequestedByCustomer
            }, cancellationToken: cancellationToken);

            return refund.Status == "succeeded" || refund.Status == "pending";
        }
    }
}

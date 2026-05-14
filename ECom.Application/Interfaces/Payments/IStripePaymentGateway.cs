namespace ECom.Application.Interfaces.Payments
{
    public interface IStripePaymentGateway
    {
        Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
            long amountCents,
            string currency,
            IReadOnlyList<string> paymentMethodTypes,
            CancellationToken cancellationToken = default);

        Task UpdatePaymentIntentAmountAsync(
            string paymentIntentId,
            long amountCents,
            CancellationToken cancellationToken = default);

        Task<bool> RefundPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    }
}

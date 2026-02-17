using ECom.Core.Entities;


namespace ECom.Core.Services
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethodId);
    }
}

using ECom.Core.Entities;

namespace ECom.Application.Interfaces.Repositories
{
    public interface ICustomerBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string id); // basketId
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string id); // basketIdIOrderRepository
    }
}

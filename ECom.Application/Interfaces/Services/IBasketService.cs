using ECom.Application.Sharing;
using ECom.Core.Entities;

namespace ECom.Application.Interfaces.Services
{
    public interface IBasketService
    {
        Task<CustomerBasket?> GetBasketAsync(string id);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<ResponseAPI> DeleteBasketAsync(string id);
    }
}

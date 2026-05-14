using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using ECom.Core.Entities;

namespace ECom.Application.Services
{
    public class BasketService : IBasketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BasketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket?> GetBasketAsync(string id)
        {
            return await _unitOfWork.CustomerBasketRepository.GetBasketAsync(id);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            return await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
        }

        public async Task<ResponseAPI> DeleteBasketAsync(string id)
        {
            var deleted = await _unitOfWork.CustomerBasketRepository.DeleteBasketAsync(id);
            return deleted
                ? new ResponseAPI(200, "Item deleted successfully")
                : new ResponseAPI(400, "Failed to delete basket");
        }
    }
}

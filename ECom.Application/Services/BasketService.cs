using ECom.Application.Common.Exceptions;
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
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return await _unitOfWork.CustomerBasketRepository.GetBasketAsync(id);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            if (basket is null || string.IsNullOrWhiteSpace(basket.Id))
                throw new BusinessException("Basket id is required.");

            basket.BasketItems ??= new List<BasketItem>();

            var updated = await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
            if (updated is null)
                throw new BusinessException("Could not save basket. Please try again.");

            return updated;
        }

        public async Task<ResponseAPI> DeleteBasketAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new ResponseAPI(400, "Basket id is required");

            await _unitOfWork.CustomerBasketRepository.DeleteBasketAsync(id);
            return new ResponseAPI(200, "Basket deleted successfully");
        }
    }
}

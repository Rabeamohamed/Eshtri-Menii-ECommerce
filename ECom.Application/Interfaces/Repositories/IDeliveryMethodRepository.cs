using ECom.Core.Entities.Order;

namespace ECom.Application.Interfaces.Repositories
{
    public interface IDeliveryMethodRepository
    {
        Task<DeliveryMethod> GetByIdAsync(int id);
        Task<IReadOnlyList<DeliveryMethod>> GetAllAsync();
    }
}

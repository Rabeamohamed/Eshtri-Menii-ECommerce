using ECom.Core.Entities.Order;

namespace ECom.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Orders> GetOrderByPaymentIntentIdAsync(string paymentIntentId);
        Task<IReadOnlyList<Orders>> GetOrdersByEmailAsync(string buyerEmail);
        Task<Orders> GetOrderByIdAsync(int id, string buyerEmail = null);
        Task AddOrderAsync(Orders order);
        Task UpdateOrderAsync(Orders order);
        Task DeleteOrderAsync(Orders order);
    }
}

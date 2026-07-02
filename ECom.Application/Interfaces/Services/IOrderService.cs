using ECom.Application.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Orders> CreateOrderAsync(OrderDto orderDto, string BuyerEmail);
        Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersForUserAsync(string BuyerEmail);
        Task<OrderToReturnDto?> GetOrderByIdAsync(int id, string BuyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
        Task<ResponseAPI> CancelOrderAsync(int orderId, string buyerEmail, bool isAdmin);

    }
}

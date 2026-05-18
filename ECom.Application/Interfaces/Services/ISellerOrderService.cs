using ECom.Application.DTO.Order;

namespace ECom.Application.Interfaces.Services
{
    public interface ISellerOrderService
    {
        Task<IReadOnlyList<OrderToReturnDto>> GetMyOrdersAsync(string sellerId);
        Task<OrderToReturnDto?> GetMyOrderByIdAsync(string sellerId, int orderId);
    }
}

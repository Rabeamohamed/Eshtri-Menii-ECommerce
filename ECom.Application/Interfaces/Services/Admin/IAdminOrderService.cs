using ECom.Application.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services.Admin
{
    public interface IAdminOrderService
    {
        // Get all orders with optional status filter
        Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersAsync(PaymentStatus? status = null);

        // Get order by id
        Task<OrderToReturnDto?> GetOrderByIdAsync(int id);

        // Update order status
        Task<ResponseAPI> UpdateOrderStatusAsync(int orderId, PaymentStatus status);

        // Get orders count by status — for dashboard stats
        Task<Dictionary<string, int>> GetOrdersCountByStatusAsync();
    }
}

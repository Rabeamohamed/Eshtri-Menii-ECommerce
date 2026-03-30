using ECom.Core.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Core.Services.Admin;
using ECom.Core.Sharing;
namespace ECom.Infrastructure.Service.Admin
{
    public class AdminOrderService : IAdminOrderService
    {
        public Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersAsync(PaymentStatus? status = null)
        {
            throw new NotImplementedException();
        }

        public Task<OrderToReturnDto> GetOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, int>> GetOrdersCountByStatusAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseAPI> UpdateOrderStatusAsync(int orderId, PaymentStatus status)
        {
            throw new NotImplementedException();
        }
    }
}

using ECom.Core.DTO.Order;
using ECom.Core.Entities.Order;
using ECom.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.Services
{
    public interface IOrderService
    {
        Task<Orders> CreateOrderAsync(OrderDto orderDto,string BuyerEmail);
        Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersForUserAsync(string BuyerEmail); 
        Task<OrderToReturnDto> GetOrderByIdAsync(int  id,string BuyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
        Task<ResponseAPI> CancelOrderAsync(int orderId, string buyerEmail, bool isAdmin);

    }
}

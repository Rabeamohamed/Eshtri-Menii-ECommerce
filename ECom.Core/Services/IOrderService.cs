using ECom.Core.DTO.Order;
using ECom.Core.Entities.Order;
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
        Task<IReadOnlyList<Orders>> GetAllOrdersForUserAsync(string BuyerEmail); 
        Task<Orders> GetOrderByIdAsync(int  id,string BuyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDealMethodAsync();
    }
}

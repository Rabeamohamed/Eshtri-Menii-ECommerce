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
        Task<Orders> CreateOrder(OrderDto orderDto)
    }
}

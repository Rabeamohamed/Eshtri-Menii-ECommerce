using ECom.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Order
{
    public record OrderToReturnDto
    {
        public int Id { get; init; }
        public string BuyerEmail { get; init; }
        public decimal SubTotal { get; init; }
        public decimal Total { get; init; }
        public DateTime OrderDate { get; init; }
        public ShippingAddress ShippingAddress { get; init; }
        public string DeliveryMethod { get; init; }
        public IReadOnlyList<OrderItemsDto> OrderItems { get; init; }
        public string Status { get; init; }
    }
}

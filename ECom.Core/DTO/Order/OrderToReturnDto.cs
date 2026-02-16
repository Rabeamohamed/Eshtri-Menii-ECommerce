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
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public string DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItemsDto> OrderItems { get; set; }
        public string Status { get; set; }
    }
}

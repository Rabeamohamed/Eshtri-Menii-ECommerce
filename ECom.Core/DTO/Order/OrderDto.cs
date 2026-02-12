
namespace ECom.Core.DTO.Order
{
    public record OrderDto
    {
        public int DeliveryMethodId { get; set; }
        public int BasketId { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
    }
}

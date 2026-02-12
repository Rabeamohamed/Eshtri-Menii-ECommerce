
namespace ECom.Core.DTO.Order
{
    public record OrderDto
    {
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
    }
}

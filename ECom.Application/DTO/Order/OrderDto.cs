
namespace ECom.Application.DTO.Order
{
    public record OrderDto
    {
        public string BasketId { get; init; }
        public int DeliveryMethodId { get; init; }
        public ShippingAddressDto ShippingAddress { get; init; }
    }
}

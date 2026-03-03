
namespace ECom.Core.DTO.Order
{
    public record ShippingAddressDto
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string City { get; init; }
        public string ZipCode { get; init; }
        public string Street { get; init; }
        public string State { get; init; }
    }
}

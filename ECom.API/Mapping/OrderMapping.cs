using AutoMapper;
using ECom.Core.DTO.Order;
using ECom.Core.Entities.Order;

namespace ECom.API.Mapping
{
    public class OrderMapping :Profile
    {
        public OrderMapping()
        {
            CreateMap<ShippingAddress,ShippingAddressDto>().ReverseMap();
        } 
    }
}

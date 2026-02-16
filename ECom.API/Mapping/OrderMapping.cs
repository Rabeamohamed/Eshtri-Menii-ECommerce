using AutoMapper;
using ECom.Core.DTO.Order;
using ECom.Core.Entities.Order;
using StackExchange.Redis;

namespace ECom.API.Mapping
{
    public class OrderMapping :Profile
    {
        public OrderMapping()
        {
            CreateMap<Orders,OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod,  
                o => o.MapFrom(s => s.DeliveryMethod.Name))
                .ReverseMap()
                ;
            CreateMap<OrderItems,OrderItemsDto>().ReverseMap();
            CreateMap<ShippingAddress,ShippingAddressDto>().ReverseMap();
        } 
    }
}

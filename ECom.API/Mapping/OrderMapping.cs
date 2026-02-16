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
            CreateMap<Order,OrderToReturnDto>().ReverseMap();
            CreateMap<OrderItems,OrderItemsDto>().ReverseMap();
            CreateMap<ShippingAddress,ShippingAddressDto>().ReverseMap();
        } 
    }
}

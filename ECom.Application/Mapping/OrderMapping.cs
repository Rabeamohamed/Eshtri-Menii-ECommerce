using AutoMapper;
using ECom.Application.DTO.Order;
using ECom.Core.Entities;
using ECom.Core.Entities.Order;


namespace ECom.Application.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Orders, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod,
                o => o.MapFrom(s => s.DeliveryMethod.Name))
                .ForMember(d => d.Total,
                    o => o.MapFrom(s => s.GetTotal()))
                .ForMember(d => d.Status,
                    o => o.MapFrom(s => s.Status.ToString()))
                .ReverseMap();

            CreateMap<OrderItems, OrderItemsDto>().ReverseMap();
            CreateMap<ShippingAddress, ShippingAddressDto>().ReverseMap();
            CreateMap<Address, ShippingAddressDto>().ReverseMap();
        }
    }
}

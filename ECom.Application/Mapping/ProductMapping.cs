using AutoMapper;
using ECom.Application.DTO;
using ECom.Application.DTO.Product;
using ECom.Core.Entities.Product;

namespace ECom.Application.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            // CreateMap<Source, Destination>();
            CreateMap<Product, ProductDto>()
                .ForMember(x=>x.CategoryName  // Map Category.Name to CategoryName in ProductDto
                , op => op.MapFrom(x => x.Category.Name)).ReverseMap();

            CreateMap<AddProductDto , Product>()
                .ForMember(p => p.Photos, op =>op.Ignore())
                .ReverseMap();

            CreateMap<UpdateProductDto, Product>()
                .ForMember(p => p.Photos, op => op.Ignore())
                .ReverseMap();

        }

    }
}

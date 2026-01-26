using AutoMapper;
using ECom.Core.DTO;
using ECom.Core.DTO.Product;
using ECom.Core.Entities.Product;

namespace ECom.API.Mapping
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

        }

    }
}

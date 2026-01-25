using AutoMapper;
using ECom.Core.DTO;
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

            CreateMap<Photo, PhotoDto>().ReverseMap();
        }

    }
}

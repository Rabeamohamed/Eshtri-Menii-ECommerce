using AutoMapper;
using ECom.Core.DTO;
using ECom.Core.Entities.Product;

namespace ECom.API.Mapping
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            // Add your mapping configurations here in the future
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();
        }
    }
}

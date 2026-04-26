using AutoMapper;
using ECom.Application.DTO.Category;
using ECom.Core.Entities.Product;

namespace ECom.Application.Mapping
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            // Add your mapping configurations here in the future
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<AddCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();

        }
    }
}

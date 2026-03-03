using AutoMapper;
using ECom.Core.DTO.Wishlist;
using ECom.Core.Entities.Product;

namespace ECom.API.Mapping
{
    public class WishlistMapping : Profile
    {
        public WishlistMapping()
        {
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt =>
                    opt.MapFrom(src => src.Product.NewPrice))
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src => src.Product.AverageRating))
                .ForMember(dest => dest.InStock,
                    opt => opt.MapFrom(src => src.Product.InStock))
                .ForMember(dest => dest.MainPhoto,
                    opt => opt.MapFrom(src => src.Product.Photos != null &&
                        src.Product.Photos.Any()
                        ? src.Product.Photos.First().ImageName
                        : null));
        }
    }
}

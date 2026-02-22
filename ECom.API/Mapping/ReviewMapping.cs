using AutoMapper;
using ECom.Core.DTO.Review;
using ECom.Core.Entities.Product;

namespace ECom.API.Mapping
{
    public class ReviewMapping : Profile
    {
        public ReviewMapping()
        {
            // Review → ReviewDto
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName,
        opt => opt.MapFrom(src => src.User != null
                    ? src.User.UserName
                    : "Unknown"));

            // CreateReviewDto → Review
            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsVerifiedPurchase,
                    opt => opt.Ignore())   // set in Service
                .ForMember(dest => dest.UserId,
                    opt => opt.Ignore());  // set in Service from JWT

            // UpdateReviewDto → Review
            CreateMap<UpdateReviewDto, Review>()
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UserId,
                    opt => opt.Ignore())   // never update userId
                .ForMember(dest => dest.ProductId,
                    opt => opt.Ignore());  // never update productId
        }
    }
}
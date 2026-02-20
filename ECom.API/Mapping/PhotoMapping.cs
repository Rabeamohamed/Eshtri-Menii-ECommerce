using AutoMapper;
using ECom.Core.DTO;
using ECom.Core.Entities.Product;

namespace ECom.Infrastructure.Data.Config
{
    public class PhotoMapping : Profile
    {
        public PhotoMapping()
        {
            // CreateMap<Source, Destination>();
            CreateMap<Photo, PhotoDto>().ReverseMap();
        }
    }
}

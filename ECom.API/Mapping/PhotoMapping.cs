using AutoMapper;
using ECom.Core.DTO;
using ECom.Core.Entities.Product;

namespace ECom.API.Mapping
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

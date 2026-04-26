using AutoMapper;
using ECom.Application.DTO;
using ECom.Core.Entities.Product;

namespace ECom.Application.Mapping
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

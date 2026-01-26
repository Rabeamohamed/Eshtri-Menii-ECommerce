using AutoMapper;
using ECom.Core.DTO;
using ECom.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECom.Infrastructure.Data.Config
{
    public class PhotoConfiguration : Profile
    {
        public PhotoConfiguration()
        {
            // CreateMap<Source, Destination>();
            CreateMap<Photo, PhotoDto>().ReverseMap();
        }
    }
}

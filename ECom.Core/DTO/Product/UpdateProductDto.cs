using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Product
{
    public record UpdateProductDto: AddProductDto
    {
        public int Id { get; set; }
    }
}

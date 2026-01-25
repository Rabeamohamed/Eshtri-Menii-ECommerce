using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO
{
    public record ProductDto
    {
        
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public List<PhotoDto> Photos { get; init; }
        public string CategoryName { get; init; }
    }

    public record PhotoDto
    {
        public string ImageName { get; init; }
        public int ProductId { get; set; }
    }
}

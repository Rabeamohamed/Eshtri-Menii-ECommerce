using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Product
{
    public record ProductDto
    {
        public int Id { get; set; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal OldPrice { get; init; }
        public decimal NewPrice { get; init; }
        public List<PhotoDto> Photos { get; init; }
        public string CategoryName { get; init; }
        public double AverageRating { get; init; }
        public int TotalReviews { get; init; }
        public int StockQuantity { get; init; }
        public bool InStock { get; init; }
    }


}

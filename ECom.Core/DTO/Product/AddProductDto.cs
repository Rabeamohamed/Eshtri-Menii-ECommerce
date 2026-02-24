using Microsoft.AspNetCore.Http;

namespace ECom.Core.DTO.Product
{
    public record AddProductDto
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal OldPrice { get; init; }
        public decimal NewPrice { get; init; }
        public int CategoryId { get; set; }
        public IFormFileCollection Photo { get; init; }
        public int StockQuantity { get; set; }
    }
}

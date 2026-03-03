
namespace ECom.Core.DTO.Product
{
    public record UpdateProductDto: AddProductDto
    {
        public int Id { get; init; }
    }
}

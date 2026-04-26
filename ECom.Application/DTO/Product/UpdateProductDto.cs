
namespace ECom.Application.DTO.Product
{
    public record UpdateProductDto: AddProductDto
    {
        public int Id { get; init; }
    }
}

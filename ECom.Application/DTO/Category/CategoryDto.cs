
namespace ECom.Application.DTO.Category
{
    public record CategoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
    }
}

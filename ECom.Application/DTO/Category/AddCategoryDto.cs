
using System.ComponentModel.DataAnnotations;

namespace ECom.Application.DTO.Category
{
    public record AddCategoryDto
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; init; }

        [MaxLength(500)]
        public string? Description { get; init; }
    }
}

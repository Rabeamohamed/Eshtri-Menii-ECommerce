
using System.ComponentModel.DataAnnotations;

namespace ECom.Core.DTO.Category
{
    public record AddCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; init; }

        [MaxLength(500)]
        public string Description { get; init; }
    }
}

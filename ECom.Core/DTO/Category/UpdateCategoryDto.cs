using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Category
{
    public record UpdateCategoryDto
    {
        [Required]
        public int Id { get; init; }

        [MaxLength(100)]
        public string Name { get; init; }

        [MaxLength(500)]
        public string Description { get; init; }
    }
}

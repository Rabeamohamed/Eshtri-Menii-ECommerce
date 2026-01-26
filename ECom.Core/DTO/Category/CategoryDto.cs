using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Category
{
    public record CategoryDto(string Name,string Description);
    public record UpdateCategoryDto(int Id,string Name,string Description);
}

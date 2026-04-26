using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Application.DTO
{
    public record PhotoDto
    {
        public string ImageName { get; init; }
        public int ProductId { get; set; }
    }
}

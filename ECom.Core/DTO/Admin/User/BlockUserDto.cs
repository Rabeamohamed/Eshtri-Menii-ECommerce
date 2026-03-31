using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Admin.User
{
    public record BlockUserDto
    {
        public string UserId { get; init; }
        public string Reason { get; init; }
    }
}

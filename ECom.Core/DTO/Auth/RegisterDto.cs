using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Auth
{
    public record RegisterDto: LoginDto
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
    }
}

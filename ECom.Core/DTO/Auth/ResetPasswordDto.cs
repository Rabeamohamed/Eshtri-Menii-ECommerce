using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Auth
{
    public record ResetPasswordDto :LoginDto
    {
        public string Token { get; set; }
    }
}

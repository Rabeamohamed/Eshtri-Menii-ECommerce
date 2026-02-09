using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.DTO.Auth
{
    public record ActiveEmailDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}

using ECom.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.Services
{
    public interface IGenerateToken
    {
        string GetAndGenerateToken(AppUser user);
    }
}

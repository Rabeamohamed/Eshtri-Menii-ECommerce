using ECom.Core.DTO;
using ECom.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Infrastructure.Service
{
    public class GenerateToken : IGenerateToken
    {
        public Task<string> GetAndGenerateToken(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}

using AutoMapper;
using ECom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductController : AdminBaseController
    {
        public AdminProductController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
    }
}

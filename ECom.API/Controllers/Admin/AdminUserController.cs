using AutoMapper;
using ECom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    public class AdminUserController : AdminBaseController
    {
        public AdminUserController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
    }
}

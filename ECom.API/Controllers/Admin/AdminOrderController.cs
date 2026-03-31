using AutoMapper;
using ECom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    public class AdminOrderController : AdminBaseController
    {
        public AdminOrderController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
    }
}

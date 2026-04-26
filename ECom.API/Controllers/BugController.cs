using AutoMapper;
using ECom.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class BugController : BaseController
    {
        public BugController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("not-found")]
        
        public async Task<ActionResult> GetNotFound()
        {
            var category = await work.CategoryRepository.GetByIdAsync(42);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet("server-error")]
        public async Task<ActionResult> GetServerError()
        {
            var category = await work.CategoryRepository.GetByIdAsync(42);
            category.Name = "";
            return Ok(category);
        }

        [HttpGet("bad-request")]

        public async Task<ActionResult> GetBadRequest()
        {
            return BadRequest();
        }


        [HttpGet("bad-request/{Id}")]
        public async Task<ActionResult> GetBadRequest(int Id)
        {
            return Ok();
        }

    }
}

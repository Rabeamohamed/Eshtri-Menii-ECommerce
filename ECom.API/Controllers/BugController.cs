using ECom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class BugController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public BugController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("not-found")]
        public async Task<ActionResult> GetNotFound()
        {
            var category = await _categoryService.GetCategoryByIdAsync(42);
            if (category is null) return NotFound();
            return Ok(category);
        }

        [HttpGet("server-error")]
        public async Task<ActionResult> GetServerError()
        {
            var category = await _categoryService.GetCategoryByIdAsync(42);
            if (category is null)
                return NotFound();

            return Ok(new { category.Id, Name = string.Empty });
        }

        [HttpGet("bad-request")]
        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }

        [HttpGet("bad-request/{Id}")]
        public ActionResult GetBadRequest(int Id)
        {
            return Ok();
        }
    }
}

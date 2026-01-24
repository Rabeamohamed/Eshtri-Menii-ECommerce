using ECom.Core.Interfaces;
using ECom.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{

    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork work) : base(work)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await work.CategoryRepository.GetAllAsync();
                if (categories is null)
                {
                    return BadRequest("No categories found.");
                }
                return Ok(categories);
            }
            catch (Exception)
            {

                return BadRequest("Something went wrong while retrieving categories.");
            }
        }
    }
}

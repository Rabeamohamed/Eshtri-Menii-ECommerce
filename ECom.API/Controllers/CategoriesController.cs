using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    //Storefront category list (read-only). Mutations use <c>AdminCategoryController</c>.</summary>
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category is null)
                return NotFound(new ResponseAPI(404, $"Category with id {id} was not found"));

            return Ok(category);
        }
    }
}

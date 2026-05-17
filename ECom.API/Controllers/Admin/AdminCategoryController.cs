using ECom.Application.DTO.Category;
using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    public class AdminCategoryController : AdminBaseController
    {
        private readonly IAdminCategoryService _categoryService;

        public AdminCategoryController(IAdminCategoryService categoryService)
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
                return NotFound(new ResponseAPI(404, "Category not found"));
            return Ok(category);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] AddCategoryDto dto)
        {
            var result = await _categoryService.CreateCategoryAsync(dto);
            return result.StatusCode switch
            {
                201 => StatusCode(201, result),
                _ => BadRequest(result),
            };
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto dto)
        {
            var result = await _categoryService.UpdateCategoryAsync(dto);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result),
            };
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result),
            };
        }
    }
}

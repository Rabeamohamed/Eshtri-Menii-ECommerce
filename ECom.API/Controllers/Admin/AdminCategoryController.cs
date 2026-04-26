using AutoMapper;
using ECom.Application.DTO.Category;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    public class AdminCategoryController : AdminBaseController
    {
        private readonly IAdminCategoryService _categoryService;
        public AdminCategoryController(IUnitOfWork work, IMapper mapper, IAdminCategoryService categoryService) : base(work, mapper)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category is null)
                {
                    return NotFound(new ResponseAPI(404, "Category not found"));
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] AddCategoryDto dto)
        {
            try
            {
                var result = await _categoryService.CreateCategoryAsync(dto);
                if (result.StatusCode != 201)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto dto)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(dto);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
    }
}

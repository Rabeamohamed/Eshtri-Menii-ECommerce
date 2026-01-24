using ECom.Core.DTO;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await work.CategoryRepository.GetByIdAsync(id);
                if (category is null)
                {
                    return BadRequest($"Category not found with this Id {id}.");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory(CategoryDto categoryDto)
        {
            try
            {
                var category = new Category()
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description
                };
                await work.CategoryRepository.AddAsync(category);

                return Ok("Category added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update-category")]
        public async Task<IActionResult> UpdateCategory( UpdateCategoryDto categoryDto)
        {
            try
            {
                //var oldCategory = await work.CategoryRepository.GetByIdAsync(categoryDto.Id);
                //if (oldCategory is null)
                //{
                //    return BadRequest($"Category not found with this Id {categoryDto.Id}.");
                //}
                var category = new Category()
                {
                    Id = categoryDto.Id,
                    Name = categoryDto.Name,
                    Description = categoryDto.Description
                };
               
                await work.CategoryRepository.UpdateAsync(category);
                return Ok("Category updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await work.CategoryRepository.GetByIdAsync(id);
                if (category is null)
                {
                    return BadRequest($"Category not found with this Id {id}.");
                }
                await work.CategoryRepository.DeleteAsync(id);
                return Ok("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}


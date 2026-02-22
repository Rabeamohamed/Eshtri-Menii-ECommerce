using AutoMapper;
using ECom.Core.DTO.Category;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Core.Sharing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{

    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork work,IMapper mapper) : base(work, mapper)
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
                    return BadRequest(new ResponseAPI(400));
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
                    return BadRequest(new ResponseAPI(400,$"Not Found Category with Id {id}"));
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
                var category = mapper.Map<Category>(categoryDto); // Using AutoMapper to map DTO to Entity
                await work.CategoryRepository.AddAsync(category);

                return Ok(new ResponseAPI(200,"Category has been Created Successfully"));
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
               
                var category = mapper.Map<Category>(categoryDto); // Using AutoMapper to map DTO to Entity
                //var oldCategory = await work.CategoryRepository.GetByIdAsync(category.Id);
                if (category is null)
                {
                    return BadRequest(new ResponseAPI(400,"there are error for update category"));
                }
                await work.CategoryRepository.UpdateAsync(category);
                return Ok(new ResponseAPI(200, "Category has been Updated Successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, "there are error for update category"));
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
                return Ok(new ResponseAPI(200, "Category Deleted Successfullu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}


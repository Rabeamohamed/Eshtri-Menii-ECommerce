using AutoMapper;
using ECom.API.Helper;
using ECom.Core.DTO.Product;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts(int? categoryId, string? sort = null)
        {
            try
            {
                var products = await work.ProductRepository
                    .GetAllAsync(categoryId, sort);

                return Ok(products);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await work.ProductRepository.
                    GetByIdAsync(id, C => C.Category, P => P.Photos);
                var result = mapper.Map<ProductDto>(product);
                if (product is null)
                {
                    return BadRequest(new ResponseAPI(404, $"Product Not found with Id {id}"));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct(AddProductDto productDto)
        {
            try
            {
                await work.ProductRepository.AddAsync(productDto);
                return Ok(new ResponseAPI(200, "Product Created Successfuly"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            try
            {
                await work.ProductRepository.UpdateAsync(updateProductDto);
                return Ok(new ResponseAPI(200, "Product Updated Successfuly"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpDelete("delete-product/{Id}")]

        public async Task<IActionResult> DeleteProduct(int Id)
        {
            try
            {
                var product = await work.ProductRepository.GetByIdAsync(
                    Id, x=>x.Category, y=>y.Photos);

                await work.ProductRepository.DeleteAsync(product);
                return Ok(new ResponseAPI(200, "Product Deleted Successfuly"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
    }
}

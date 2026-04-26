using AutoMapper;
using ECom.Application.DTO.Product;
using ECom.Application.Interfaces;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductParams productParams)
        {
            try
            {
                var products = await work.ProductRepository
                    .GetAllAsync( productParams);

                var totalCount = await work.ProductRepository
                    .CountAsync(); // Get total count of prodcuts for pagination 

                return Ok(new Pagination<ProductDto>(productParams.PageNumber,productParams.PageSize, totalCount,products));
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

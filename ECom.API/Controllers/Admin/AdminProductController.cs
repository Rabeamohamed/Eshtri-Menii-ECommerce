using AutoMapper;
using ECom.API.Helper;
using ECom.Core.DTO.Product;
using ECom.Core.Entities.Product;
using ECom.Core.Interfaces;
using ECom.Core.Services.Admin;
using ECom.Core.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductController : AdminBaseController
    {
        private readonly IAdminProductService _productService;
        public AdminProductController(IUnitOfWork work, IMapper mapper, IAdminProductService productService) : base(work, mapper)
        {
            _productService = productService;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductParams productParams)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync(productParams);
                var totalCount = await _productService.GetTotalCountAsync();
                return Ok(new Pagination<ProductDto>(
                    productParams.PageNumber,
                    productParams.PageSize,
                    totalCount,
                    products));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product is null)
                {
                    return NotFound(new ResponseAPI(404, $"Product not found with Id {id}"));
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromForm] AddProductDto dto)
        {
            try
            {
                var result = await _productService.CreateProductAsync(dto);
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
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDto dto)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(dto);
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
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
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

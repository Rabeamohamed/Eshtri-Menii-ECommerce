using ECom.Application.DTO.Product;
using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    public class AdminProductController : AdminBaseController
    {
        private readonly IAdminProductService _productService;

        public AdminProductController(IAdminProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductParams productParams)
        {
            var products = await _productService.GetAllProductsAsync(productParams);
            var totalCount = await _productService.GetTotalCountAsync(productParams);
            return Ok(new Pagination<ProductDto>(
                productParams.PageNumber,
                productParams.PageSize,
                totalCount,
                products));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
                return NotFound(new ResponseAPI(404, $"Product not found with Id {id}"));
            return Ok(product);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromForm] AddProductDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);
            return result.StatusCode switch
            {
                201 => StatusCode(201, result),
                _ => BadRequest(result),
            };
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDto dto)
        {
            var result = await _productService.UpdateProductAsync(dto);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result),
            };
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result),
            };
        }
    }
}

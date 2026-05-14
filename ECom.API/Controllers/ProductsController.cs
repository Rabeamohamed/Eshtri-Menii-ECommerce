using ECom.Application.DTO.Product;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductParams productParams)
        {
            var page = await _productService.GetProductsPageAsync(productParams);
            return Ok(page);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result is null)
                return NotFound(new ResponseAPI(404, $"Product not found with id {id}"));
            return Ok(result);
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct(AddProductDto productDto)
        {
            var result = await _productService.CreateProductAsync(productDto);
            return result.StatusCode switch
            {
                201 => StatusCode(201, result),
                _ => BadRequest(result)
            };
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var result = await _productService.UpdateProductAsync(updateProductDto);
            return result.StatusCode switch
            {
                200 => Ok(result),
                _ => BadRequest(result)
            };
        }

        [HttpDelete("delete-product/{Id}")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var result = await _productService.DeleteProductAsync(Id);
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
    }
}

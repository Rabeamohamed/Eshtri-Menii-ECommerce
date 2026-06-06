using ECom.Application.DTO.Product;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECom.API.Controllers.Seller
{
    public class SellerProductController : SellerBaseController
    {
        private readonly ISellerProductService _productService;

        public SellerProductController(ISellerProductService productService)
        {
            _productService = productService;
        }

        private string GetSellerId()
        {
            var sellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return sellerId ?? string.Empty;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetMyProducts([FromQuery] ProductParams productParams)
        {
            var sellerId = GetSellerId();
            var products = await _productService.GetMyProductsAsync(sellerId, productParams);
            var totalCount = await _productService.GetMyTotalCountAsync(sellerId, productParams);
            return Ok(new Pagination<ProductDto>(
                productParams.PageNumber,
                productParams.PageSize,
                totalCount,
                products));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetMyProductById(int id)
        {
            var sellerId = GetSellerId();
            var product = await _productService.GetMyProductByIdAsync(sellerId, id);
            if (product is null)
                return NotFound(new ResponseAPI(404, $"Product not found with Id {id} or you do not have permission to view it."));
            return Ok(product);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMyProduct([FromForm] AddProductDto dto)
        {
            // Validate incoming form data
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ResponseAPI(400, "Invalid product data: " + string.Join(", ", errors)));
            }

            var sellerId = GetSellerId();
            var result = await _productService.CreateMyProductAsync(sellerId, dto);
            return result.StatusCode switch
            {
                201 => StatusCode(201, result),
                _ => BadRequest(result),
            };
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateMyProduct([FromForm] UpdateProductDto dto)
        {
            var sellerId = GetSellerId();
            var result = await _productService.UpdateMyProductAsync(sellerId, dto);
            return result.StatusCode switch
            {
                200 => Ok(result),
                403 => StatusCode(403, result),
                404 => NotFound(result),
                _ => BadRequest(result),
            };
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMyProduct(int id)
        {
            var sellerId = GetSellerId();
            var result = await _productService.DeleteMyProductAsync(sellerId, id);
            return result.StatusCode switch
            {
                200 => Ok(result),
                403 => StatusCode(403, result),
                404 => NotFound(result),
                _ => BadRequest(result),
            };
        }
    }
}

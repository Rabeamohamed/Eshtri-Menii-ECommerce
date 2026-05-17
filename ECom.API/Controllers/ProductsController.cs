using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    /// <summary>Storefront product catalog (read-only). Admin mutations use AdminProductController.</summary>
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
    }
}

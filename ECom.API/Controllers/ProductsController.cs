using AutoMapper;
using ECom.API.Helper;
using ECom.Core.DTO;
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
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await work.ProductRepository.
                    GetAllAsync(C => C.Category, P => P.Photos);

                var result = mapper.Map<List<ProductDto>>(products);

                if (products is null)
                {
                    return BadRequest(new ResponseAPI(400));
                }
                return Ok(result);
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
        //[HttpPost("create-product")]
        //public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        //{
        //    try
        //    {
        //        var product = mapper.Map<Product>(productDto);
        //        await work.ProductRepository.AddAsync(product);
        //        await work.CommitAsync();
        //        return Ok(new ResponseAPI(200, "Product Created Successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
}

using ECom.Application.DTO.Product;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Pagination<ProductDto>> GetProductsPageAsync(ProductParams productParams);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ResponseAPI> CreateProductAsync(AddProductDto productDto);
        Task<ResponseAPI> UpdateProductAsync(UpdateProductDto updateProductDto);
        Task<ResponseAPI> DeleteProductAsync(int id);
    }
}

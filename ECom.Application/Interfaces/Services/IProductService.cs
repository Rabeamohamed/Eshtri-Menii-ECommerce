using ECom.Application.DTO.Product;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Pagination<ProductDto>> GetProductsPageAsync(ProductParams productParams);
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}

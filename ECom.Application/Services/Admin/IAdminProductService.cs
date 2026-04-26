using ECom.Application.DTO.Product;
using ECom.Application.Sharing;

namespace ECom.Application.Services.Admin
{
    public interface IAdminProductService
    {
        Task <IReadOnlyList<ProductDto>> GetAllProductsAsync(ProductParams productParams);
        Task<int> GetTotalCountAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ResponseAPI> CreateProductAsync(AddProductDto dto);
        Task<ResponseAPI> UpdateProductAsync(UpdateProductDto dto);
        Task<ResponseAPI> DeleteProductAsync(int id);
    }
}

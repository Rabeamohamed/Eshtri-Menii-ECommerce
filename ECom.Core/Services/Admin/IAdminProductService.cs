using ECom.Core.DTO.Product;
using ECom.Core.Sharing;

namespace ECom.Core.Services.Admin
{
    public interface IAdminProductService
    {
        Task <IReadOnlyList<ProductDto>> GetAllProductsAsync(ProductParams productParams);
        Task<int> GetTotalCount();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ResponseAPI> CreateProductAsync(AddProductDto dto);
        Task<ResponseAPI> UpdateProductAsync(UpdateProductDto dto);
        Task<ResponseAPI> DeleteProductAsync(int id);
    }
}

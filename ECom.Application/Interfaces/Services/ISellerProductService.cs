using ECom.Application.DTO.Product;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services
{
    public interface ISellerProductService
    {
        Task<IReadOnlyList<ProductDto>> GetMyProductsAsync(string sellerId, ProductParams productParams);
        Task<int> GetMyTotalCountAsync(string sellerId, ProductParams productParams);
        Task<ProductDto?> GetMyProductByIdAsync(string sellerId, int id);
        Task<ResponseAPI> CreateMyProductAsync(string sellerId, AddProductDto dto);
        Task<ResponseAPI> UpdateMyProductAsync(string sellerId, UpdateProductDto dto);
        Task<ResponseAPI> DeleteMyProductAsync(string sellerId, int id);
    }
}

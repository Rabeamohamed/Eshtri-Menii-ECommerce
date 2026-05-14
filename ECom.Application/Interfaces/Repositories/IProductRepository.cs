using ECom.Application.DTO.Product;
using ECom.Core.Entities.Product;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<int> CountAsync(ProductParams productParams);
        Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams);
        Task<bool> AddAsync(AddProductDto  productDto);
        Task<bool> UpdateAsync(UpdateProductDto productDto);
        Task DeleteAsync(Product product);

    }
}

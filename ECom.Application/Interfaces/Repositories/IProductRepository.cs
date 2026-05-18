using ECom.Application.DTO.Product;
using ECom.Core.Entities.Product;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<int> CountAsync(ProductParams productParams);
        Task<int> CountAsync(); // Count all products (no filter)
        Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams);
        Task<bool> AddAsync(AddProductDto productDto);
        Task<bool> UpdateAsync(UpdateProductDto productDto);
        Task<bool> UpdateAsync(Product product); // entity-level update
        Task DeleteAsync(Product product);

    }
}

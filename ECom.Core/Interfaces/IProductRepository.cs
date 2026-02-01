using ECom.Core.DTO.Product;
using ECom.Core.Entities.Product;
using ECom.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Additional Method that specialized for Product only
        // Add Product-specific methods here
        Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams); // Method Arguments / Parameters	camelCase	public void AddCustomer(string customerName);
        Task<bool> AddAsync(AddProductDto  productDto);
        Task<bool> UpdateAsync(UpdateProductDto productDto);
        Task DeleteAsync(Product product);

    }
}

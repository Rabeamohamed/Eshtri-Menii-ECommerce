using ECom.Application.DTO.Product;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;

namespace ECom.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AutoMapper.IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Pagination<ProductDto>> GetProductsPageAsync(ProductParams productParams)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(productParams);
            var totalCount = await _unitOfWork.ProductRepository.CountAsync(productParams);
            return new Pagination<ProductDto>(
                productParams.PageNumber,
                productParams.PageSize,
                totalCount,
                products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, c => c.Category, p => p.Photos);
            if (product is null) return null;
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ResponseAPI> CreateProductAsync(AddProductDto productDto)
        {
            var ok = await _unitOfWork.ProductRepository.AddAsync(productDto);
            return ok
                ? new ResponseAPI(201, "Product created successfully")
                : new ResponseAPI(400, "Failed to create product");
        }

        public async Task<ResponseAPI> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var ok = await _unitOfWork.ProductRepository.UpdateAsync(updateProductDto);
            return ok
                ? new ResponseAPI(200, "Product updated successfully")
                : new ResponseAPI(400, "Failed to update product");
        }

        public async Task<ResponseAPI> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, x => x.Category, y => y.Photos);
            if (product is null)
                return new ResponseAPI(404, $"Product not found with id {id}");

            await _unitOfWork.ProductRepository.DeleteAsync(product);
            return new ResponseAPI(200, "Product deleted successfully");
        }
    }
}

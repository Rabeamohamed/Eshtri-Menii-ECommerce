using AutoMapper;
using ECom.Application.DTO.Product;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services;
using ECom.Application.Sharing;

namespace ECom.Application.Services
{
    public class SellerProductService : ISellerProductService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SellerProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseAPI> CreateMyProductAsync(string sellerId, AddProductDto dto)
        {
            if (dto is null)
                return new ResponseAPI(400, "Product data is required");
            if (dto.CategoryId <= 0)
                return new ResponseAPI(400, "A valid category is required");

            dto.SellerId = sellerId; // Force the seller ID to the current user
            var ok = await _unitOfWork.ProductRepository.AddAsync(dto);
            return ok
                ? new ResponseAPI(201, "Product created successfully")
                : new ResponseAPI(400, "Failed to create product");
        }

        public async Task<ResponseAPI> DeleteMyProductAsync(string sellerId, int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, p => p.Category, p => p.Photos);
            if (product is null)
                return new ResponseAPI(404, "Product not found");

            if (product.SellerId != sellerId)
                return new ResponseAPI(403, "You do not have permission to delete this product");

            await _unitOfWork.ProductRepository.DeleteAsync(product);
            return new ResponseAPI(200, "Product deleted successfully");
        }

        public async Task<ProductDto?> GetMyProductByIdAsync(string sellerId, int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, p => p.Category, product => product.Photos);
            if (product is null || product.SellerId != sellerId)
                return null;
                
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IReadOnlyList<ProductDto>> GetMyProductsAsync(string sellerId, ProductParams productParams)
        {
            productParams.SellerId = sellerId;
            var products = await _unitOfWork.ProductRepository.GetAllAsync(productParams);
            return products.ToList();
        }

        public async Task<int> GetMyTotalCountAsync(string sellerId, ProductParams productParams)
        {
            productParams.SellerId = sellerId;
            return await _unitOfWork.ProductRepository.CountAsync(productParams);
        }

        public async Task<ResponseAPI> UpdateMyProductAsync(string sellerId, UpdateProductDto dto)
        {
            if (dto is null)
                return new ResponseAPI(400, "Product data is required");
            if (dto.CategoryId <= 0)
                return new ResponseAPI(400, "A valid category is required");

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.Id);
            if (product is null)
                return new ResponseAPI(404, "Product not found");

            if (product.SellerId != sellerId)
                return new ResponseAPI(403, "You do not have permission to update this product");

            dto.SellerId = sellerId; // Ensure seller ID cannot be hijacked
            var ok = await _unitOfWork.ProductRepository.UpdateAsync(dto);
            return ok
                ? new ResponseAPI(200, "Product updated successfully")
                : new ResponseAPI(400, "Failed to update product");
        }
    }
}

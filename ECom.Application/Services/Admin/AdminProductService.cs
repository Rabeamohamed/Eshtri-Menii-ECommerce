using AutoMapper;
using ECom.Application.DTO.Product;
using ECom.Application.Sharing;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services.Admin;

namespace ECom.Application.Services.Admin
{
    public class AdminProductService : IAdminProductService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AdminProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseAPI> CreateProductAsync(AddProductDto dto)
        {
            if (dto is null)
            {
                return new ResponseAPI(400, "Product data is required");
            }
            await _unitOfWork.ProductRepository.AddAsync(dto);
            return new ResponseAPI(201, "Product created successfully");
        }

        public async Task<ResponseAPI> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, p => p.Category, p => p.Photos);
            if (product is null)
            {
                return new ResponseAPI(404, "Product not found");
            }
            await _unitOfWork.ProductRepository.DeleteAsync(id);
            return new ResponseAPI(200, "Product Deleted successfully");
        }

        public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(ProductParams productParams)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(productParams);
            return products.ToList();
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, p => p.Category, product => product.Photos);
            if (product is null)
                return null;
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<int> GetTotalCountAsync()
            => await _unitOfWork.ProductRepository.CountAsync();

        public async Task<ResponseAPI> UpdateProductAsync(UpdateProductDto dto)
        {
            if (dto is null)
            {
                return new ResponseAPI(400, "Product data is required");
            }
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.Id);
            if (product is null)
            {
                return new ResponseAPI(404, "Product not found");
            }

            await _unitOfWork.ProductRepository.UpdateAsync(dto);
            return new ResponseAPI(200, "Product updated successfully");
        }
    }
}

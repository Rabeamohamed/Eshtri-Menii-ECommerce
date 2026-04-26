using AutoMapper;
using ECom.Application.DTO.Category;
using ECom.Core.Entities.Product;
using ECom.Application.Sharing;
using ECom.Application.Interfaces.Repositories;
using ECom.Application.Interfaces.Services.Admin;
namespace ECom.Infrastructure.Service.Admin
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AdminCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ResponseAPI> CreateCategoryAsync(AddCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);

            await _unitOfWork.CategoryRepository.AddAsync(category);
            var result = await _unitOfWork.SaveChangesAsync();
            if(result <= 0)
            {
                return new ResponseAPI(400, "Failed to create category");
            }
            return new ResponseAPI(201, "Category created successfully");

        }

        public async Task<ResponseAPI> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if(category is null)
            {
                return new ResponseAPI(404, "Category not found");
            }
            await _unitOfWork.CategoryRepository.DeleteAsync(id);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new ResponseAPI(400, "Failed to delete category");
            }
            return new ResponseAPI(200, "Category deleted successfully");
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
        }

        public Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if(category is null)
            {
                return null;
            }
            return _mapper.Map<Task<CategoryDto>>(category);
        }

        public async Task<ResponseAPI> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(dto.Id);
            if(category is null)
            {
                return new ResponseAPI(404, "Category not found");
            }
            _mapper.Map(dto, category);
            await _unitOfWork.CategoryRepository.UpdateAsync(category);
            var result = await _unitOfWork.SaveChangesAsync();
            if(result <= 0)
            {
                return new ResponseAPI(400, "Failed to update category");
            }
            return new ResponseAPI(200, "Category updated successfully");
        }
    }
}

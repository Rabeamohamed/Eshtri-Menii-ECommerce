using AutoMapper;
using ECom.Core.DTO.Category;
using ECom.Core.Interfaces;
using ECom.Core.Services.Admin;
using ECom.Core.Sharing;
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
        public Task<ResponseAPI> CreateCategoryAsync(AddCategoryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseAPI> DeleteCategoryAsync(int id)
        {
            throw new NotImplementedException();
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

        public Task<ResponseAPI> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            throw new NotImplementedException();
        }
    }
}

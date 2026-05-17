using ECom.Application.DTO.Category;
using ECom.Application.Sharing;

namespace ECom.Application.Interfaces.Services.Admin
{
    public interface IAdminCategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<ResponseAPI> CreateCategoryAsync(AddCategoryDto dto);
        Task<ResponseAPI> UpdateCategoryAsync(UpdateCategoryDto dto);
        Task<ResponseAPI> DeleteCategoryAsync(int id);
    }
}

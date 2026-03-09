
using ECom.Core.DTO.Category;
using ECom.Core.Sharing;

namespace ECom.Core.Interfaces.Admin
{
    public interface IAdminCategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<ResponseAPI> CreateCategoryAsync(AddCategoryDto dto);
        Task<ResponseAPI> UpdateCategoryAsync(UpdateCategoryDto dto);
        Task<ResponseAPI> DeleteCategoryAsync(int id);

    }
}

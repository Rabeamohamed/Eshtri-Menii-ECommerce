using ECom.Application.DTO.Category;

namespace ECom.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
    }
}

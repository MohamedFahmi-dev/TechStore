using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Category;

namespace TechStore.BLL.Interface;

public interface ICategoryService
{

    Task<Result<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<Result<IEnumerable<CategoryDto>>> GetActiveAsync();
    Task<Result<IEnumerable<CategoryDto>>> GetRootCategoriesAsync();
    Task<Result<IEnumerable<CategoryDto>>> GetChildrenAsync(int parentId);
    Task<Result<IEnumerable<CategoryDto>>> GetTreeAsync();
    Task<Result<CategoryDto>> GetByIdAsync(int id);
    Task<Result<CategoryDto>> GetBySlugAsync(string slug);
    Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto);
    Task<Result<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<Result> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}


using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Brand;

namespace TechStore.BLL.Interface;

public interface IBrandService
{
    Task<Result<IEnumerable<BrandDto>>> GetAllAsync();
    Task<Result<IEnumerable<BrandDto>>> GetActiveAsync();
    Task<Result<BrandDto>> GetByIdAsync(int id);
    Task<Result<BrandDto>> GetBySlugAsync(string slug);
    Task<Result<BrandDto>> CreateAsync(CreateBrandDto dto);
    Task<Result<BrandDto>> UpdateAsync(int id, UpdateBrandDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result> ExistsAsync(int id);
}


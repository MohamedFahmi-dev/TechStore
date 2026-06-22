using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Category;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        var categories = await _uow.Categories.GetTableNoTracking()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .ToListAsync();
        return Result<IEnumerable<CategoryDto>>.Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetActiveAsync()
    {
        var categories = await _uow.Categories.GetTableNoTracking()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Where(c => c.IsActive)
            .ToListAsync();
        return Result<IEnumerable<CategoryDto>>.Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetRootCategoriesAsync()
    {
        var categories = await _uow.Categories.GetTableNoTracking()
            .Include(c => c.SubCategories)
            .Where(c => c.ParentId == null && c.IsActive)
            .ToListAsync();
        return Result<IEnumerable<CategoryDto>>.Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetChildrenAsync(int parentId)
    {
        var categories = await _uow.Categories.GetTableNoTracking()
            .Include(c => c.SubCategories)
            .Where(c => c.ParentId == parentId && c.IsActive)
            .ToListAsync();
        return Result<IEnumerable<CategoryDto>>.Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetTreeAsync()
    {
        var allCats = await _uow.Categories.GetTableNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
            
        var roots = allCats.Where(c => c.ParentId == null).ToList();
        var dtos = roots.Select(r => BuildTree(r, allCats)).ToList();
        return Result<IEnumerable<CategoryDto>>.Ok(dtos);
    }

    private CategoryDto BuildTree(Category node, List<Category> allCategories)
    {
        var dto = _mapper.Map<CategoryDto>(node);
        var children = allCategories.Where(c => c.ParentId == node.Id).OrderBy(c => c.DisplayOrder).ToList();
        if (children.Any())
        {
            dto.Children = children.Select(c => BuildTree(c, allCategories)).ToList();
        }
        return dto;
    }

    public async Task<Result<CategoryDto>> GetByIdAsync(int id)
    {
        var category = await _uow.Categories.GetTableNoTracking()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
            
        if (category == null) return Error.NotFound("Category.NotFound");
        return Result<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
    }

    public async Task<Result<CategoryDto>> GetBySlugAsync(string slug)
    {
        var category = await _uow.Categories.GetTableNoTracking()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Slug == slug);
            
        if (category == null) return Error.NotFound("Category.NotFound");
        return Result<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
    }

    public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        if (await ExistsAsync(0, dto.Slug)) return Error.Validation("Category.SlugExists", "A category with this slug already exists.");
        
        var category = new Category
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            ParentId = dto.ParentId,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive
        };

        await _uow.Categories.AddAsync(category);
        await _uow.SaveChangesAsync();

        return await GetByIdAsync(category.Id);
    }

    public async Task<Result<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        if (await ExistsAsync(id, dto.Slug)) return Error.Validation("Category.SlugExists", "A category with this slug already exists.");

        var category = await _uow.Categories.GetByIdAsync(id);
        if (category == null) return Error.NotFound("Category.NotFound");

        category.Name = dto.Name;
        category.Slug = dto.Slug;
        category.Description = dto.Description;
        category.ImageUrl = dto.ImageUrl;
        category.ParentId = dto.ParentId;
        category.DisplayOrder = dto.DisplayOrder;
        category.IsActive = dto.IsActive;

        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var category = await _uow.Categories.GetByIdAsync(id);
        if (category == null) return Error.NotFound("Category.NotFound");

        var hasChildren = await _uow.Categories.FindAsync(c => c.ParentId == id);
        if (hasChildren.Any()) return Error.Validation("Category.HasChildren", "Cannot delete a category that has subcategories.");

        _uow.Categories.Delete(category);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _uow.Categories.GetTableNoTracking().AnyAsync(c => c.Id == id);
    }
    
    private async Task<bool> ExistsAsync(int excludeId, string slug)
    {
        return await _uow.Categories.GetTableNoTracking().AnyAsync(c => c.Id != excludeId && c.Slug == slug);
    }
}

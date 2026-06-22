using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Brand;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Services;

public class BrandService : IBrandService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public BrandService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<BrandDto>>> GetAllAsync()
    {
        var brands = await _uow.Brands.GetTableNoTracking()
            .Include(b => b.Products)
            .ToListAsync();
            
        return Result<IEnumerable<BrandDto>>.Ok(_mapper.Map<IEnumerable<BrandDto>>(brands));
    }

    public async Task<Result<BrandDto>> GetByIdAsync(int id)
    {
        var brand = await _uow.Brands.GetTableNoTracking()
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id);
            
        if (brand is null) return Error.NotFound("Brand.NotFound", "Brand not found.");

        return Result<BrandDto>.Ok(_mapper.Map<BrandDto>(brand));
    }

    public async Task<Result<IEnumerable<BrandDto>>> GetActiveAsync()
    {
        var brands = await _uow.Brands.GetTableNoTracking()
            .Include(b => b.Products)
            .Where(b => b.IsActive)
            .ToListAsync();
            
        return Result<IEnumerable<BrandDto>>.Ok(_mapper.Map<IEnumerable<BrandDto>>(brands));
    }

    public async Task<Result<BrandDto>> GetBySlugAsync(string slug)
    {
        var brand = await _uow.Brands.GetTableNoTracking()
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Slug == slug);
            
        if (brand is null)
            return Error.NotFound("Brand.NotFound", $"Brand with slug {slug} not found.");

        return Result<BrandDto>.Ok(_mapper.Map<BrandDto>(brand));
    }

    public async Task<Result<BrandDto>> CreateAsync(CreateBrandDto dto)
    {
        var brand = new Brand
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            IsActive = dto.IsActive
        };
        
        await _uow.Brands.AddAsync(brand);
        await _uow.SaveChangesAsync();
        
        return Result<BrandDto>.Ok(_mapper.Map<BrandDto>(brand));
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var brand = await _uow.Brands.GetByIdAsync(id);
        if (brand is null)
            return Error.NotFound("Brand.NotFound", "Brand not found.");
            
        _uow.Brands.Delete(brand);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> ExistsAsync(int id)
    {
        var exists = await _uow.Brands.GetTableNoTracking().AnyAsync(b => b.Id == id && b.IsActive);
        if (!exists) return Error.NotFound("Brand.NotFound", "Brand not found.");
        return Result.Ok();
    }

    public async Task<Result<BrandDto>> UpdateAsync(int id, UpdateBrandDto dto)
    {
        var brand = await _uow.Brands.GetByIdAsync(id);
        if (brand is null)
            return Error.NotFound("Brand.NotFound", "Brand not found.");
            
        brand.Name = dto.Name;
        brand.Slug = dto.Slug;
        brand.Description = dto.Description;
        brand.LogoUrl = dto.LogoUrl;
        brand.IsActive = dto.IsActive;
        
        _uow.Brands.Update(brand);
        await _uow.SaveChangesAsync();
        
        // Return mapped so it includes product counts properly by loading it fresh
        return await GetByIdAsync(brand.Id);
    }
}

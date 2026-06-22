using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Homepage;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Services;

public class HomepageService : IHomepageService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public HomepageService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<HomepageSectionDto>>> GetAllSectionsAsync()
    {
        var sections = await _uow.HomepageSections.GetTableNoTracking()
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Brand)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();

        return Result<IEnumerable<HomepageSectionDto>>.Ok(_mapper.Map<IEnumerable<HomepageSectionDto>>(sections));
    }

    public async Task<Result<IEnumerable<HomepageSectionDto>>> GetActiveSectionsAsync()
    {
        var sections = await _uow.HomepageSections.GetTableNoTracking()
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Brand)
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();

        return Result<IEnumerable<HomepageSectionDto>>.Ok(_mapper.Map<IEnumerable<HomepageSectionDto>>(sections));
    }

    public async Task<Result<HomepageSectionDto>> GetSectionByIdAsync(int id)
    {
        var section = await _uow.HomepageSections.GetTableNoTracking()
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Brand)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (section is null)
            return Error.NotFound("Section.NotFound", "Section not found.");

        return Result<HomepageSectionDto>.Ok(_mapper.Map<HomepageSectionDto>(section));
    }

    public async Task<Result<HomepageSectionDto>> GetSectionBySlugAsync(string slug)
    {
        var section = await _uow.HomepageSections.GetTableNoTracking()
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Brand)
            .FirstOrDefaultAsync(s => s.Slug == slug);

        if (section is null)
            return Error.NotFound("Section.NotFound", $"Section '{slug}' not found.");

        return Result<HomepageSectionDto>.Ok(_mapper.Map<HomepageSectionDto>(section));
    }

    public async Task<Result<HomepageSectionDto>> CreateSectionAsync(CreateHomepageSectionDto dto)
    {
        var section = new HomepageSection
        {
            Title = dto.Title,
            Slug = dto.Slug,
            SectionType = dto.SectionType,
            IsActive = dto.IsActive,
            DisplayOrder = dto.DisplayOrder
        };

        await _uow.HomepageSections.AddAsync(section);
        await _uow.SaveChangesAsync();
        return Result<HomepageSectionDto>.Ok(_mapper.Map<HomepageSectionDto>(section));
    }

    public async Task<Result<HomepageSectionDto>> UpdateSectionAsync(int id, UpdateHomepageSectionDto dto)
    {
        var section = await _uow.HomepageSections.GetByIdAsync(id);
        if (section is null)
            return Error.NotFound("Section.NotFound", "Section not found.");

        section.Title = dto.Title;
        section.IsActive = dto.IsActive;
        section.DisplayOrder = dto.DisplayOrder;

        await _uow.SaveChangesAsync();
        return await GetSectionByIdAsync(id);
    }

    public async Task<Result> SetSectionActiveAsync(int id, bool isActive)
    {
        var section = await _uow.HomepageSections.GetByIdAsync(id);
        if (section is null)
            return Error.NotFound("Section.NotFound", "Section not found.");

        section.IsActive = isActive;
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteSectionAsync(int id)
    {
        var section = await _uow.HomepageSections.GetByIdAsync(id);
        if (section is null)
            return Error.NotFound("Section.NotFound", "Section not found.");

        _uow.HomepageSections.Delete(section);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<HomepageSectionDto>> AddItemAsync(int sectionId, AddSectionItemDto dto)
    {
        var section = await _uow.HomepageSections.GetByIdAsync(sectionId);
        if (section is null)
            return Error.NotFound("Section.NotFound", "Section not found.");

        var item = new HomepageSectionItem
        {
            HomepageSectionId = sectionId,
            ProductId = dto.ProductId,
            DisplayOrder = dto.DisplayOrder,
            IsHighlighted = dto.IsHighlighted
        };

        await _uow.HomepageSectionItems.AddAsync(item);
        await _uow.SaveChangesAsync();
        return await GetSectionByIdAsync(sectionId);
    }

    public async Task<Result> RemoveItemAsync(int itemId)
    {
        var item = await _uow.HomepageSectionItems.GetByIdAsync(itemId);
        if (item is null)
            return Error.NotFound("SectionItem.NotFound", "Section item not found.");

        _uow.HomepageSectionItems.Delete(item);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> SetItemHighlightedAsync(int itemId, bool isHighlighted)
    {
        var item = await _uow.HomepageSectionItems.GetByIdAsync(itemId);
        if (item is null)
            return Error.NotFound("SectionItem.NotFound", "Section item not found.");

        item.IsHighlighted = isHighlighted;
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> ReorderSectionsAsync(IEnumerable<ReorderItemDto> order)
    {
        var sectionIds = order.Select(o => o.Id).ToList();
        var sections = await _uow.HomepageSections.FindAsync(s => sectionIds.Contains(s.Id));
        
        foreach (var item in order)
        {
            var section = sections.FirstOrDefault(s => s.Id == item.Id);
            if (section is not null)
                section.DisplayOrder = item.DisplayOrder;
        }
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> ReorderItemsAsync(int sectionId, IEnumerable<ReorderItemDto> order)
    {
        var itemIds = order.Select(o => o.Id).ToList();
        var sectionItems = await _uow.HomepageSectionItems.FindAsync(i => itemIds.Contains(i.Id) && i.HomepageSectionId == sectionId);
        
        foreach (var item in order)
        {
            var sectionItem = sectionItems.FirstOrDefault(i => i.Id == item.Id);
            if (sectionItem is not null)
                sectionItem.DisplayOrder = item.DisplayOrder;
        }
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }
}

using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Homepage;

namespace TechStore.BLL.Interface;

public interface IHomepageService
{
    Task<Result<IEnumerable<HomepageSectionDto>>> GetActiveSectionsAsync();
    Task<Result<HomepageSectionDto>> GetSectionBySlugAsync(string slug);

    Task<Result<IEnumerable<HomepageSectionDto>>> GetAllSectionsAsync();
    Task<Result<HomepageSectionDto>> GetSectionByIdAsync(int id);
    Task<Result<HomepageSectionDto>> CreateSectionAsync(CreateHomepageSectionDto dto);
    Task<Result<HomepageSectionDto>> UpdateSectionAsync(int id, UpdateHomepageSectionDto dto);
    Task<Result> DeleteSectionAsync(int id);
    Task<Result> SetSectionActiveAsync(int id, bool isActive);
    Task<Result> ReorderSectionsAsync(IEnumerable<ReorderItemDto> order);

    Task<Result<HomepageSectionDto>> AddItemAsync(int sectionId, AddSectionItemDto dto);
    Task<Result> RemoveItemAsync(int itemId);
    Task<Result> ReorderItemsAsync(int sectionId, IEnumerable<ReorderItemDto> order);
    Task<Result> SetItemHighlightedAsync(int itemId, bool isHighlighted);
}


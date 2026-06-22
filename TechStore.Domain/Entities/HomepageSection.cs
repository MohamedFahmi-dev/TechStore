using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class HomepageSection : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string SectionType { get; set; } = "Products";
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public ICollection<HomepageSectionItem> Items { get; set; } = new List<HomepageSectionItem>();
}

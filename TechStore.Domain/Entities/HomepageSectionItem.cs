using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class HomepageSectionItem : BaseEntity
{
    public int HomepageSectionId { get; set; }
    public int ProductId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsHighlighted { get; set; }

    public HomepageSection? HomepageSection { get; set; }
    public Product? Product { get; set; }
}

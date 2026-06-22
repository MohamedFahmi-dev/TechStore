using TechStore.Domain.Common;
using TechStore.Domain.Enums;

namespace TechStore.Domain.Entities;

public class RelatedProduct : BaseEntity
{
    public int ProductId { get; set; }
    public int RelatedProductId { get; set; }
    public RelatedProductType RelationType { get; set; } = RelatedProductType.Related;
    public int DisplayOrder { get; set; }

    public Product? Product { get; set; }
    public Product? RelatedItem { get; set; }
}

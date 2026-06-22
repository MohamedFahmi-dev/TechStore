using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class ProductSpec : BaseEntity
{
    public int ProductId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public Product? Product { get; set; }
}

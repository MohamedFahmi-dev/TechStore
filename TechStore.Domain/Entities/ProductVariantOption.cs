using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class ProductVariantOption : BaseEntity
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public Product? Product { get; set; }
    public ICollection<ProductVariantOptionValue> Values { get; set; } = new List<ProductVariantOptionValue>();
}

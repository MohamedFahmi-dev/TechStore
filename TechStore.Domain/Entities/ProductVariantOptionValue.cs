using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class ProductVariantOptionValue : BaseEntity
{
    public int ProductVariantOptionId { get; set; }
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ProductVariantOption? ProductVariantOption { get; set; }
    public ICollection<ProductVariantValue> ProductVariantValues { get; set; } = new List<ProductVariantValue>();
}

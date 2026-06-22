using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class ProductVariantValue : BaseEntity
{
    public int ProductVariantId { get; set; }
    public int ProductVariantOptionValueId { get; set; }

    public ProductVariant? ProductVariant { get; set; }
    public ProductVariantOptionValue? ProductVariantOptionValue { get; set; }
}

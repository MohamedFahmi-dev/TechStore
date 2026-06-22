using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }

    public Product? Product { get; set; }
}

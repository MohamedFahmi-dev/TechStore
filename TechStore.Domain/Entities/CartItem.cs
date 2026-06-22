using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class CartItem : BaseEntity
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Cart? Cart { get; set; }
    public Product? Product { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}

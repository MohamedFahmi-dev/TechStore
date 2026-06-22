using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? VariantLabel { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}

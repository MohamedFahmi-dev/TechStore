using TechStore.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    [ConcurrencyCheck]
    public int StockQuantity { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    public Product? Product { get; set; }
    public ICollection<ProductVariantValue> ProductVariantValues { get; set; } = new List<ProductVariantValue>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

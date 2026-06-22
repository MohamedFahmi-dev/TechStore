using TechStore.Domain.Common;
using TechStore.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.Entities;

public class Product : BaseEntity
{
    public int BrandId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? DiscountPrice { get; set; }
    [ConcurrencyCheck]
    public int StockQuantity { get; set; }
    public decimal EffectivePrice => DiscountPrice ?? BasePrice;
    public bool HasVariants { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; } = true;
    public int SoldCount { get; set; }
    public ProductLabel Label { get; set; } = ProductLabel.None;
    public string? Condition { get; set; }
    public string? WarrantyInfo { get; set; }

    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductSpec> Specs { get; set; } = new List<ProductSpec>();
    public ICollection<ProductVariantOption> VariantOptions { get; set; } = new List<ProductVariantOption>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
    public ICollection<RelatedProduct> RelatedProducts { get; set; } = new List<RelatedProduct>();
    public ICollection<RelatedProduct> RelatedToProducts { get; set; } = new List<RelatedProduct>();
    public ICollection<HomepageSectionItem> HomepageSectionItems { get; set; } = new List<HomepageSectionItem>();
}

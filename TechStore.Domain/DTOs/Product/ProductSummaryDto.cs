using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Product
{
    public class ProductSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal EffectivePrice => DiscountPrice ?? BasePrice;
        public string? MainImageUrl { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool HasVariants { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }
        public int StockQuantity { get; set; }
        public ProductLabel Label { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}

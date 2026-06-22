using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Product
{
    public class CreateProductDto
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
        public int StockQuantity { get; set; }
        public bool HasVariants { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; } = true;
        public ProductLabel Label { get; set; } = ProductLabel.None;
        public string? Condition { get; set; }
        public string? WarrantyInfo { get; set; }
    }
    public class UpdateProductDto : CreateProductDto { }

}

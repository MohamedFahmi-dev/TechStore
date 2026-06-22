namespace TechStore.Domain.DTOs.Product
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<ProductVariantValueDto> Values { get; set; } = Enumerable.Empty<ProductVariantValueDto>();
    }
}
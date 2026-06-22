namespace TechStore.Domain.DTOs.Product
{
    public class ProductDetailDto : ProductSummaryDto
    {
        public string? Description { get; set; }
        public string? WarrantyInfo { get; set; }
        public string? Condition { get; set; }
        public int SoldCount { get; set; }
        public IEnumerable<ProductImageDto> Images { get; set; } = Enumerable.Empty<ProductImageDto>();
        public IEnumerable<ProductSpecGroupDto> Specs { get; set; } = Enumerable.Empty<ProductSpecGroupDto>();
        public IEnumerable<ProductVariantOptionDto> VariantOptions { get; set; } = Enumerable.Empty<ProductVariantOptionDto>();
        public IEnumerable<ProductVariantDto> Variants { get; set; } = Enumerable.Empty<ProductVariantDto>();
        public IEnumerable<ProductSummaryDto> RelatedProducts { get; set; } = Enumerable.Empty<ProductSummaryDto>();
    }
}

namespace TechStore.Domain.DTOs.Product
{
    public class ProductVariantOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public IEnumerable<ProductVariantOptionValueDto> Values { get; set; } = Enumerable.Empty<ProductVariantOptionValueDto>();
    }

}
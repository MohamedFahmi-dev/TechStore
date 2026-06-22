namespace TechStore.Domain.DTOs.Product
{
    public class ProductVariantOptionValueDto
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}

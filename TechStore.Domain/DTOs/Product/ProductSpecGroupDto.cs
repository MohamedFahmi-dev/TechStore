namespace TechStore.Domain.DTOs.Product
{
    public class ProductSpecGroupDto
    {
        public string GroupName { get; set; } = string.Empty;
        public IEnumerable<ProductSpecDto> Specs { get; set; } = Enumerable.Empty<ProductSpecDto>();
    }
}
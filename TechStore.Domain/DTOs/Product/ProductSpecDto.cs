namespace TechStore.Domain.DTOs.Product
{
    public class ProductSpecDto
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}

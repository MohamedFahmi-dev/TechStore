namespace TechStore.Domain.DTOs.Product
{
    public class AddProductImageDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }
}

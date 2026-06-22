namespace TechStore.Domain.DTOs.Homepage
{
    public class CreateHomepageSectionDto
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string SectionType { get; set; } = "Products";
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }
}

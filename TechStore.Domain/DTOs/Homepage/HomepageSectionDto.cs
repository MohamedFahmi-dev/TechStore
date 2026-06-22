namespace TechStore.Domain.DTOs.Homepage
{
    using TechStore.Domain.DTOs.Product;

    public class HomepageSectionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string SectionType { get; set; } = "Products";
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public IEnumerable<HomepageSectionItemDto> Items { get; set; } = Enumerable.Empty<HomepageSectionItemDto>();
    }
}

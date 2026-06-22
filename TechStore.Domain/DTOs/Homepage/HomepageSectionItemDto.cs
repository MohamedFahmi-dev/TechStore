namespace TechStore.Domain.DTOs.Homepage
{
    using TechStore.Domain.DTOs.Product;

    public class HomepageSectionItemDto
    {
        public int Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsHighlighted { get; set; }
        public ProductSummaryDto? Product { get; set; }
    }
}

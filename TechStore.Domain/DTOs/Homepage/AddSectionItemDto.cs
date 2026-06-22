namespace TechStore.Domain.DTOs.Homepage
{
    public class AddSectionItemDto
    {
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsHighlighted { get; set; }
    }
}

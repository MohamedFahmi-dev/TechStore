namespace TechStore.Domain.DTOs.Review
{
    public class CreateReviewDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }        // 1-5
        public string? Comment { get; set; }
    }
}

namespace TechStore.Domain.DTOs.Wishlist
{
    using TechStore.Domain.DTOs.Product;

    public class WishlistDto
    {
        public int UserId { get; set; }
        public IEnumerable<ProductSummaryDto> Products { get; set; } = Enumerable.Empty<ProductSummaryDto>();
        public int Count => Products.Count();
    }
}

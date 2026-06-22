namespace TechStore.Domain.DTOs.Cart
{
    public class AddCartItemDto
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }

}

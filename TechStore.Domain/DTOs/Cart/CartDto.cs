namespace TechStore.Domain.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public IEnumerable<CartItemDto> Items { get; set; } = Enumerable.Empty<CartItemDto>();
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal Subtotal => Items.Sum(i => i.LineTotal);
    }
}

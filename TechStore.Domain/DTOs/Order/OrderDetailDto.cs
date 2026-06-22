namespace TechStore.Domain.DTOs.Order
{
    public class OrderDetailDto : OrderSummaryDto
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public string? Notes { get; set; }
        public string? CouponCode { get; set; }
        public OrderAddressDto Address { get; set; } = new();
        public IEnumerable<OrderItemDto> Items { get; set; } = Enumerable.Empty<OrderItemDto>();
        public OrderPaymentDto? Payment { get; set; }
    }
}

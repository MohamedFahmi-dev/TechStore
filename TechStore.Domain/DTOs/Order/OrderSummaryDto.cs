using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Order
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}

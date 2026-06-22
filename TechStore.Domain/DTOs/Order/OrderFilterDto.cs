using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Order
{
    public class OrderFilterDto
    {
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? UserId { get; set; }
    }

}

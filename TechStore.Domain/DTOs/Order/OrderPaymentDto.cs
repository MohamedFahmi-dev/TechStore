using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Order
{
    public class OrderPaymentDto
    {
        public string Method { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }
    }

}

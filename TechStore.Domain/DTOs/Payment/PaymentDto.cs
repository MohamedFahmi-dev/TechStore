namespace TechStore.Domain.DTOs.Payment
{
    using TechStore.Domain.Enums;

    public class PaymentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Method { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}

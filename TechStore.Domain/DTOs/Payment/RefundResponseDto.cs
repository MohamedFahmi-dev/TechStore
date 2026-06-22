namespace TechStore.Domain.DTOs.Payment;

public class RefundResponseDto
{
    public string RefundId { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}

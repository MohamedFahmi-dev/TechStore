using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.DTOs.Payment;

public class ConfirmPaymentDto
{
    [Required]
    public string PaymentIntentId { get; set; } = string.Empty;

    [Required]
    public string PaymentMethodId { get; set; } = string.Empty;
}

public class ConfirmPaymentResponseDto
{
    public string PaymentIntentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

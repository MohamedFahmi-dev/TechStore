using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.DTOs.Payment;

public class CreatePaymentIntentDto
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; } = "EGP";

    public string? Description { get; set; }

    public string? IdempotencyKey { get; set; }
}

public class CreatePaymentIntentResponseDto
{
    public string ClientSecret { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

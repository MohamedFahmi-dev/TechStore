using TechStore.Domain.Common;
using TechStore.Domain.Enums;

namespace TechStore.Domain.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public string Method { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaidAt { get; set; }

    public Order? Order { get; set; }
}

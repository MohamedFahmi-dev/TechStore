using TechStore.Domain.Common;
using TechStore.Domain.Enums;

namespace TechStore.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public int AddressId { get; set; }
    public int? CouponId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    public ApplicationUser? User { get; set; }
    public Address? Address { get; set; }
    public Coupon? Coupon { get; set; }
    public Payment? Payment { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

namespace TechStore.Domain.DTOs.Coupon
{
    using TechStore.Domain.Enums;

    public class CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CouponDiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public bool IsActive { get; set; }
    }
}

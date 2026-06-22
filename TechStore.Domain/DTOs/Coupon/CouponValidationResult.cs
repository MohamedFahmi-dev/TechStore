namespace TechStore.Domain.DTOs.Coupon
{
    public class CouponValidationResult
    {
        public bool IsValid { get; set; }
        public string? Error { get; set; }
        public CouponDto? Coupon { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? CouponId { get; set; }
    }
}

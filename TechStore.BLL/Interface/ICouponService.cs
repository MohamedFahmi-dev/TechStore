using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Coupon;

namespace TechStore.BLL.Interface;

public interface ICouponService
{
    Task<Result<IEnumerable<CouponDto>>> GetAllAsync();
    Task<Result<CouponDto>> GetByIdAsync(int id);
    Task<Result<CouponDto>> GetByCodeAsync(string code);
    Task<Result<CouponValidationResult>> ValidateAsync(string code, decimal orderSubtotal);
    Task<Result<decimal>> ApplyDiscountAsync(string code, decimal orderSubtotal);
    Task<Result<CouponDto>> CreateAsync(CreateCouponDto dto);
    Task<Result<CouponDto>> UpdateAsync(int id, UpdateCouponDto dto);
    Task<Result> DeleteAsync(int id);

    Task<Result> IncrementUsageAsync(int couponId);
}


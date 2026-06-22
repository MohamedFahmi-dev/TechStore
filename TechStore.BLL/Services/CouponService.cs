using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Coupon;
using TechStore.Domain.Entities;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Services;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CouponService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CouponDto>>> GetAllAsync()
    {
        var coupons = await _uow.Coupons.GetTableNoTracking().ToListAsync();
        return Result<IEnumerable<CouponDto>>.Ok(_mapper.Map<IEnumerable<CouponDto>>(coupons));
    }

    public async Task<Result<CouponDto>> GetByIdAsync(int id)
    {
        var coupon = await _uow.Coupons.GetTableNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (coupon is null)
            return Error.NotFound("Coupon.NotFound", "Coupon not found.");
        return Result<CouponDto>.Ok(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<Result<CouponDto>> GetByCodeAsync(string code)
    {
        var coupon = await _uow.Coupons.GetTableNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code);
        if (coupon is null)
            return Error.NotFound("Coupon.NotFound", $"Coupon '{code}' not found.");
        return Result<CouponDto>.Ok(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<Result<CouponValidationResult>> ValidateAsync(string code, decimal orderSubtotal)
    {
        var coupon = await _uow.Coupons.GetTableNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive);

        if (coupon is null)
            return Error.Validation("Coupon.Invalid", "Invalid or inactive coupon code.");

        var now = DateTime.UtcNow;
        if (now < coupon.StartsAt || now > coupon.EndsAt)
            return Error.Validation("Coupon.Expired", "This coupon is not valid at this time.");

        if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            return Error.Validation("Coupon.Exhausted", "This coupon has reached its usage limit.");

        if (coupon.MinimumOrderAmount.HasValue && orderSubtotal < coupon.MinimumOrderAmount.Value)
            return Error.Validation("Coupon.MinimumNotMet", $"Minimum order amount is {coupon.MinimumOrderAmount:C}.");

        var discount = coupon.DiscountType == CouponDiscountType.Percentage
            ? orderSubtotal * (coupon.DiscountValue / 100)
            : coupon.DiscountValue;

        return Result<CouponValidationResult>.Ok(new CouponValidationResult
        {
            IsValid = true,
            DiscountAmount = Math.Min(discount, orderSubtotal),
            CouponId = coupon.Id
        });
    }

    public async Task<Result<decimal>> ApplyDiscountAsync(string code, decimal orderSubtotal)
    {
        var result = await ValidateAsync(code, orderSubtotal);
        if (!result.IsSuccess) return Result<decimal>.Fail(result.Errors.ToList());
        return Result<decimal>.Ok(result.Value.DiscountAmount);
    }

    public async Task<Result> IncrementUsageAsync(int couponId)
    {
        var coupon = await _uow.Coupons.GetByIdAsync(couponId);
        if (coupon is null)
            return Error.NotFound("Coupon.NotFound", "Coupon not found.");

        coupon.UsedCount++;
        _uow.Coupons.Update(coupon);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<CouponDto>> CreateAsync(CreateCouponDto dto)
    {
        var coupon = new Coupon
        {
            Code = dto.Code.ToUpperInvariant(),
            Description = dto.Description,
            DiscountType = dto.DiscountType,
            DiscountValue = dto.DiscountValue,
            MinimumOrderAmount = dto.MinimumOrderAmount,
            UsageLimit = dto.UsageLimit,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            IsActive = dto.IsActive
        };

        await _uow.Coupons.AddAsync(coupon);
        await _uow.SaveChangesAsync();
        return Result<CouponDto>.Ok(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<Result<CouponDto>> UpdateAsync(int id, UpdateCouponDto dto)
    {
        var coupon = await _uow.Coupons.GetByIdAsync(id);
        if (coupon is null)
            return Error.NotFound("Coupon.NotFound", "Coupon not found.");

        coupon.IsActive = dto.IsActive;
        await _uow.SaveChangesAsync();
        return Result<CouponDto>.Ok(_mapper.Map<CouponDto>(coupon));
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var coupon = await _uow.Coupons.GetByIdAsync(id);
        if (coupon is null)
            return Error.NotFound("Coupon.NotFound", "Coupon not found.");

        _uow.Coupons.Delete(coupon);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }
}

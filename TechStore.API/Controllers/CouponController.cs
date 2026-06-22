using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Coupon;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Coupon.Prefix)]
public class CouponController : AppControllerBase
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _couponService.GetAllAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Coupon.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _couponService.GetByIdAsync(id);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Coupon.GetByCode)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var result = await _couponService.GetByCodeAsync(code);
        return Handle(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCouponDto dto)
    {
        var result = await _couponService.CreateAsync(dto);
        return Handle(result, created: true);
    }

    [HttpPut(ApiRoutes.Coupon.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponDto dto)
    {
        var result = await _couponService.UpdateAsync(id, dto);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Coupon.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _couponService.DeleteAsync(id);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Coupon.Validate)]
    [Authorize]
    public async Task<IActionResult> ValidateAndApply([FromQuery] string code, [FromQuery] decimal cartTotal)
    {
        var result = await _couponService.ApplyDiscountAsync(code, cartTotal);
        return Handle(result);
    }
}

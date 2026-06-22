using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Review;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Review.Prefix)]
public class ReviewController : AppControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet(ApiRoutes.Review.GetById)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _reviewService.GetByIdAsync(id);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Review.ProductReviews)]
    public async Task<IActionResult> GetByProduct(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _reviewService.GetByProductAsync(productId, page, pageSize);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Review.MyReviews)]
    [Authorize]
    public async Task<IActionResult> GetByUser([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _reviewService.GetByUserAsync(CurrentUserId, page, pageSize);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Review.Summary)]
    public async Task<IActionResult> GetSummary(int productId)
    {
        var result = await _reviewService.GetSummaryAsync(productId);
        return Handle(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
        var result = await _reviewService.CreateAsync(CurrentUserId, dto);
        return Handle(result, created: true);
    }

    [HttpPut(ApiRoutes.Review.GetById)]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto)
    {
        var result = await _reviewService.UpdateAsync(id, CurrentUserId, dto);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Review.GetById)]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _reviewService.DeleteAsync(id, CurrentUserId);
        return Handle(result);
    }

    [HttpDelete("admin/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDelete(int id)
    {
        var result = await _reviewService.AdminDeleteAsync(id);
        return Handle(result);
    }

    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _reviewService.ApproveAsync(id);
        return Handle(result);
    }
}

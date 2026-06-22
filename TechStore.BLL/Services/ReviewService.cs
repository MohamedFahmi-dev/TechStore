using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Review;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<ReviewDto>> GetByIdAsync(int reviewId)
    {
        var review = await _uow.Reviews.GetTableNoTracking()
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review is null)
            return Error.NotFound("Review.NotFound", "Review not found.");

        return Result<ReviewDto>.Ok(_mapper.Map<ReviewDto>(review));
    }

    public async Task<Result<PagedResult<ReviewDto>>> GetByProductAsync(int productId, int page = 1, int pageSize = 10)
    {
        var query = _uow.Reviews.GetTableNoTracking()
            .Include(r => r.User)
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.Id);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Result<PagedResult<ReviewDto>>.Ok(new PagedResult<ReviewDto>
        {
            Items = _mapper.Map<List<ReviewDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<Result<PagedResult<ReviewDto>>> GetByUserAsync(int userId, int page = 1, int pageSize = 10)
    {
        var query = _uow.Reviews.GetTableNoTracking()
            .Include(r => r.User)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Id);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Result<PagedResult<ReviewDto>>.Ok(new PagedResult<ReviewDto>
        {
            Items = _mapper.Map<List<ReviewDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<Result<ReviewSummaryDto>> GetSummaryAsync(int productId)
    {
        var reviews = await _uow.Reviews.GetTableNoTracking()
            .Where(r => r.ProductId == productId && r.IsApproved)
            .ToListAsync();

        var summary = new ReviewSummaryDto
        {
            ProductId = productId,
            TotalCount = reviews.Count,
            AverageRating = reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0,
            RatingBreakdown = reviews
                .GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return Result<ReviewSummaryDto>.Ok(summary);
    }

    public async Task<Result<ReviewDto>> CreateAsync(int userId, CreateReviewDto dto)
    {
        // Check if already reviewed
        var alreadyReviewed = await _uow.Reviews.GetTableNoTracking()
            .AnyAsync(r => r.UserId == userId && r.ProductId == dto.ProductId);
        if (alreadyReviewed)
            return Error.Validation("Review.AlreadyReviewed", "You have already reviewed this product.");
        
        // Check if user has purchased and received the product
        var hasPurchased = await _uow.Orders.GetTableNoTracking()
            .AnyAsync(o => o.UserId == userId 
                        && o.OrderStatus == TechStore.Domain.Enums.OrderStatus.Delivered 
                        && o.Items.Any(i => i.ProductId == dto.ProductId));
        if (!hasPurchased)
            return Error.Validation("Review.NotPurchased", "You can only review products you have purchased.");

        var review = new Review
        {
            UserId = userId,
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            IsApproved = false // require admin moderation
        };

        await _uow.Reviews.AddAsync(review);
        await _uow.SaveChangesAsync();

        return await GetByIdAsync(review.Id);
    }

    public async Task<Result<ReviewDto>> UpdateAsync(int reviewId, int userId, UpdateReviewDto dto)
    {
        var review = await _uow.Reviews.FindAsync(r => r.Id == reviewId && r.UserId == userId);
        var entity = review.FirstOrDefault();

        if (entity is null)
            return Error.NotFound("Review.NotFound", "Review not found or unauthorized.");

        entity.Rating = dto.Rating;
        entity.Comment = dto.Comment;

        await _uow.SaveChangesAsync();
        return await GetByIdAsync(reviewId);
    }

    public async Task<Result> DeleteAsync(int reviewId, int userId)
    {
        var review = await _uow.Reviews.FindAsync(r => r.Id == reviewId && r.UserId == userId);
        var entity = review.FirstOrDefault();

        if (entity is null)
            return Error.NotFound("Review.NotFound", "Review not found or unauthorized.");

        _uow.Reviews.Delete(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> AdminDeleteAsync(int reviewId)
    {
        var review = await _uow.Reviews.GetByIdAsync(reviewId);
        if (review is null)
            return Error.NotFound("Review.NotFound", "Review not found.");

        _uow.Reviews.Delete(review);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> ApproveAsync(int reviewId)
    {
        var review = await _uow.Reviews.GetByIdAsync(reviewId);
        if (review is null)
            return Error.NotFound("Review.NotFound", "Review not found.");

        review.IsApproved = true;
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<bool>> CanUserReviewAsync(int userId, int productId)
    {
        var alreadyReviewed = await _uow.Reviews.GetTableNoTracking()
            .AnyAsync(r => r.UserId == userId && r.ProductId == productId);

        if (alreadyReviewed)
            return Result<bool>.Ok(false);

        var hasPurchased = await _uow.Orders.GetTableNoTracking()
            .AnyAsync(o => o.UserId == userId 
                        && o.OrderStatus == TechStore.Domain.Enums.OrderStatus.Delivered 
                        && o.Items.Any(i => i.ProductId == productId));

        return Result<bool>.Ok(hasPurchased);
    }
}

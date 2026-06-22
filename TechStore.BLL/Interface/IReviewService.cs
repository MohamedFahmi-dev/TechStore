using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Review;

namespace TechStore.BLL.Interface;

public interface IReviewService
{
    Task<Result<PagedResult<ReviewDto>>> GetByProductAsync(int productId, int page = 1, int pageSize = 10);
    Task<Result<PagedResult<ReviewDto>>> GetByUserAsync(int userId, int page = 1, int pageSize = 10);
    Task<Result<ReviewDto>> GetByIdAsync(int reviewId);
    Task<Result<ReviewSummaryDto>> GetSummaryAsync(int productId);
    Task<Result<ReviewDto>> CreateAsync(int userId, CreateReviewDto dto);
    Task<Result<ReviewDto>> UpdateAsync(int reviewId, int userId, UpdateReviewDto dto);
    Task<Result> DeleteAsync(int reviewId, int userId);
    Task<Result> AdminDeleteAsync(int reviewId);
    Task<Result> ApproveAsync(int reviewId);
    Task<Result<bool>> CanUserReviewAsync(int userId, int productId);

}


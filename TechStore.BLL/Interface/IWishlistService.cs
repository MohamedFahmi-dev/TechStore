using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Wishlist;

namespace TechStore.BLL.Interface;

public interface IWishlistService
{
    Task<Result<WishlistDto>> GetByUserAsync(int userId);
    Task<Result<WishlistDto>> AddProductAsync(int userId, int productId);
    Task<Result<WishlistDto>> RemoveProductAsync(int userId, int productId);
    Task<Result> ClearAsync(int userId);
    Task<Result<bool>> IsInWishlistAsync(int userId, int productId);
    Task<Result<int>> GetCountAsync(int userId);

}


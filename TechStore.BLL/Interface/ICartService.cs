using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Cart;

namespace TechStore.BLL.Interface;

public interface ICartService
{
    Task<Result<CartDto>> GetByUserAsync(int userId);
    Task<Result<CartDto>> AddItemAsync(int userId, AddCartItemDto dto);
    Task<Result<CartDto>> UpdateItemQuantityAsync(int userId, int cartItemId, int quantity);
    Task<Result<CartDto>> RemoveItemAsync(int userId, int cartItemId);
    Task<Result> ClearAsync(int userId);
    Task<Result<int>> GetItemCountAsync(int userId);
    Task<Result<decimal>> GetTotalAsync(int userId);

    Task<Result> DeactivateAsync(int userId);
}


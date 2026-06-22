using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Wishlist.Prefix)]
[Authorize]
public class WishlistController : AppControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWishlist()
    {
        var result = await _wishlistService.GetByUserAsync(CurrentUserId);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Wishlist.Product)]
    public async Task<IActionResult> AddProduct(int productId)
    {
        var result = await _wishlistService.AddProductAsync(CurrentUserId, productId);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Wishlist.Product)]
    public async Task<IActionResult> RemoveProduct(int productId)
    {
        var result = await _wishlistService.RemoveProductAsync(CurrentUserId, productId);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Wishlist.Clear)]
    public async Task<IActionResult> ClearWishlist()
    {
        var result = await _wishlistService.ClearAsync(CurrentUserId);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Wishlist.Count)]
    public async Task<IActionResult> GetCount()
    {
        var result = await _wishlistService.GetCountAsync(CurrentUserId);
        return Handle(result);
    }
}

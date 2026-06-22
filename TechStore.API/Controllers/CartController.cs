using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Cart;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Cart.Prefix)]
[Authorize]
public class CartController : AppControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cartService.GetByUserAsync(CurrentUserId);
        return Handle(result);
    }

    [HttpGet("total")]
    public async Task<IActionResult> GetTotal()
    {
        var result = await _cartService.GetTotalAsync(CurrentUserId);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Cart.Items)]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
    {
        var result = await _cartService.AddItemAsync(CurrentUserId, dto);
        return Handle(result);
    }

    [HttpPut(ApiRoutes.Cart.ItemById)]
    public async Task<IActionResult> UpdateItemQuantity(int itemId, [FromBody] UpdateCartItemQuantityDto dto)
    {
        var result = await _cartService.UpdateItemQuantityAsync(CurrentUserId, itemId, dto.Quantity);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Cart.ItemById)]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        var result = await _cartService.RemoveItemAsync(CurrentUserId, itemId);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Cart.Clear)]
    public async Task<IActionResult> ClearCart()
    {
        var result = await _cartService.ClearAsync(CurrentUserId);
        return Handle(result);
    }
}

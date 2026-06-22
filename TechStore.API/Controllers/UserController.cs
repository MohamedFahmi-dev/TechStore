using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.User.Prefix)]
public class UserController : AppControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet(ApiRoutes.User.Profile)]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetProfileAsync(CurrentUserId);
        return Handle(result);
    }

    [HttpPut(ApiRoutes.User.UpdateProfile)]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] TechStore.Domain.DTOs.User.UpdateProfileRequestDto request)
    {
        var result = await _userService.UpdateProfileAsync(CurrentUserId, request);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.User.Addresses)]
    [Authorize]
    public async Task<IActionResult> GetAddresses()
    {
        var result = await _userService.GetAddressesAsync(CurrentUserId);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.User.AddressById)]
    [Authorize]
    public async Task<IActionResult> GetAddressById(int addressId)
    {
        var result = await _userService.GetAddressByIdAsync(CurrentUserId, addressId);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.User.Addresses)]
    [Authorize]
    public async Task<IActionResult> AddAddress([FromBody] TechStore.Domain.DTOs.User.AddAddressRequestDto request)
    {
        var result = await _userService.AddAddressAsync(CurrentUserId, request);
        return Handle(result, created: true);
    }

    [HttpPut(ApiRoutes.User.AddressById)]
    [Authorize]
    public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] TechStore.Domain.DTOs.User.UpdateAddressRequestDto request)
    {
        request.AddressId = addressId;
        var result = await _userService.UpdateAddressAsync(CurrentUserId, request);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.User.AddressById)]
    [Authorize]
    public async Task<IActionResult> DeleteAddress(int addressId)
    {
        var result = await _userService.DeleteAddressAsync(CurrentUserId, addressId);
        return Handle(result);
    }

    [HttpPut(ApiRoutes.User.SetDefaultAddress)]
    [Authorize]
    public async Task<IActionResult> SetDefaultAddress([FromBody] TechStore.Domain.DTOs.User.SetDefaultAddressRequestDto request)
    {
        var result = await _userService.SetDefaultAddressAsync(CurrentUserId, request);
        return Handle(result);
    }
}

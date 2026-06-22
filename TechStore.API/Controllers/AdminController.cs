using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Admin;
using TechStore.Domain.Enums;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Admin.Prefix)]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController : AppControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet(ApiRoutes.Admin.Users)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _adminService.GetAllUsersAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Admin.UserById)]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var result = await _adminService.GetUserByIdAsync(userId);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Admin.CreateUser)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
    {
        var result = await _adminService.CreateUserAsync(request);
        return Handle(result, created: true);
    }

    [HttpPut(ApiRoutes.Admin.UpdateUser)]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequestDto request)
    {
        request.UserId = userId;
        var result = await _adminService.UpdateUserAsync(request);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Admin.AssignRole)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequestDto request)
    {
        var result = await _adminService.AssignRoleAsync(request);
        return Handle(result);
    }

    [HttpPut(ApiRoutes.Admin.ActivateUser)]
    public async Task<IActionResult> ActivateUser(int userId)
    {
        var result = await _adminService.ActivateUserAsync(userId);
        return Handle(result);
    }

    [HttpPut(ApiRoutes.Admin.DeactivateUser)]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        var result = await _adminService.DeactivateUserAsync(userId);
        return Handle(result);
    }
}

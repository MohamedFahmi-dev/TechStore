using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using TechStore.API.Base;
using TechStore.BLL.Auth;
using TechStore.Domain.DTOs.Auth;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Auth.Prefix)]
[EnableRateLimiting("AuthLimiter")]
public class AuthController : AppControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost(ApiRoutes.Auth.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return Handle(result, created: true);
    }

    [HttpPost(ApiRoutes.Auth.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Auth.RefreshToken)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Auth.Logout)]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.LogoutAsync(request);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Auth.ForgotPassword)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Auth.VerifyResetCode)]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequestDto request)
    {
        var result = await _authService.VerifyResetCodeAsync(request);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Auth.ResetPassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Auth.ChangePassword)]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null)
            return UnauthorizedResult();

        var result = await _authService.ChangePasswordAsync(email, request);
        return Handle(result);
    }
}

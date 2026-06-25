using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Auth;
using TechStore.Domain.Entities;
using TechStore.Domain.Enums;
using TechStore.DAL.UnitOfWork;
using TechStore.BLL.Interfaces;

namespace TechStore.BLL.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _emailService;

    public AuthService(UserManager<ApplicationUser> userManager,
            IRefreshTokenService refreshTokenService,
            IJwtService jwtService,
            IUnitOfWork uow,
            IEmailService emailService)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
        _jwtService = jwtService;
        _uow = uow;
        _emailService = emailService;
    }
    public async Task<Result<TokenResponseDTO>> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Error.InvalidCredentials("User.InvalidCredentials", "Invalid email or password");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Error.InvalidCredentials("User.InvalidCredentials", "Invalid email or password");

        var accessToken = await _jwtService.GenerateTokenAsync(user);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

        return new TokenResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<Result<TokenResponseDTO>> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user already exists
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email || 
                                     u.PhoneNumber == request.PhoneNumber);
        
        if (existingUser is not null)
            return Error.Validation("User.AlreadyExists", "User with this email or phone number already exists.");
        
        var user = new ApplicationUser
        {
            UserName = request.Username, // Identity requires usernames without spaces. Email is safe.
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Name = $"{request.FirstName} {request.LastName}",
            PhoneNumber = request.PhoneNumber,
        };

        await _uow.BeginTransactionAsync();

        var identityResult = await _userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            await _uow.RollbackTransactionAsync();
            return identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        }

        var roleResult = await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
        if (!roleResult.Succeeded)
        {
            await _uow.RollbackTransactionAsync();
            return Error.Failure("RoleAssignmentFailed", "Registration failed.");
        }

        await _uow.CommitTransactionAsync();

        var accessToken = await _jwtService.GenerateTokenAsync(user);
        var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

        return new TokenResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
    
    public async Task<Result<TokenResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var userId = await _refreshTokenService.GetUserIdFromValidRefreshTokenAsync(request.RefreshToken);
        if (userId is null)
            return Error.InvalidCredentials("User.InvalidCredentials", "Refresh Token Is Invalid");
            
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Error.InvalidCredentials("User.InvalidCredentials");
            
        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
        var newAccessToken = await _jwtService.GenerateTokenAsync(user);
        var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

        return new TokenResponseDTO
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
    
    public async Task<Result> LogoutAsync(RefreshTokenRequestDto request)
    {
        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
        return Result.Ok();
    }
    
    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result.Ok();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendEmailAsync(user.Email, "Reset Password", $"Your reset code is: {token}");
        return Result.Ok();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Error.Validation("User.NotFound", "User not found.");

        var result = await _userManager.ResetPasswordAsync(user, request.Code, request.NewPassword);
        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList());

        return Result.Ok();
    }

    public async Task<Result> VerifyResetCodeAsync(VerifyResetCodeRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Error.Validation("User.NotFound", "User not found.");
        
        var provider = _userManager.Options.Tokens.PasswordResetTokenProvider;
        var isValid = await _userManager.VerifyUserTokenAsync(user, provider, "ResetPassword", request.Code);
        if (!isValid) return Error.Validation("Token.Invalid", "Invalid or expired reset code.");
        
        return Result.Ok();
    }
    
    public async Task<Result> ChangePasswordAsync(string userEmail, ChangePasswordRequestDto passwordDTO)
    {
        if (passwordDTO.NewPassword != passwordDTO.ConfirmNewPassword)
            return Result.Fail(Error.InvalidCredentials("User.InvalidCredentials", "New Password And Confirmation Password Do Not Match"));

        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null)
            return Result.Fail(Error.Unauthorized("User.Unauthorized"));

        var result = await _userManager.ChangePasswordAsync(user, passwordDTO.CurrentPassword, passwordDTO.NewPassword);
        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList());

        return Result.Ok();
    }
    
    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }
}

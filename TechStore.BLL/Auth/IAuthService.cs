using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Auth;

namespace TechStore.BLL.Auth;

public interface IAuthService
{
    Task<Result<TokenResponseDTO>> RegisterAsync(RegisterRequestDto request);
    Task<Result<TokenResponseDTO>> LoginAsync(LoginRequestDto request);
    Task<Result<TokenResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<Result> LogoutAsync(RefreshTokenRequestDto request);
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<Result> VerifyResetCodeAsync(VerifyResetCodeRequestDto request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<Result> ChangePasswordAsync(string userEmail, ChangePasswordRequestDto request);
    Task<bool> EmailExistsAsync(string email);
}


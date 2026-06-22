namespace TechStore.BLL.Auth;

public interface IRefreshTokenService
{
    Task<string> GenerateRefreshTokenAsync(int userId);
    Task<string?> GetUserIdFromValidRefreshTokenAsync(string Token);
    Task RevokeRefreshTokenAsync(string refreshToken);
}


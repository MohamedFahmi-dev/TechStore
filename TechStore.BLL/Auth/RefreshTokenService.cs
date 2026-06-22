using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using TechStore.DAL.Context;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly TechDbContext _context;

    public RefreshTokenService(TechDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateRefreshTokenAsync(int userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var RefreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
        };
        _context.RefreshTokens.Add(RefreshToken);
        await _context.SaveChangesAsync();
        return token;

    }

    public async Task<string?> GetUserIdFromValidRefreshTokenAsync(string Token)
    {
        var refreshtoken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == Token && !rt.IsRevoked);
        if (refreshtoken is null || refreshtoken.ExpiresOnUtc < DateTime.UtcNow)
            return null;
        return refreshtoken.UserId.ToString();
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var Token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (Token is not null)
        {
            Token.IsRevoked = true;
            await _context.SaveChangesAsync();

        }
    }
}


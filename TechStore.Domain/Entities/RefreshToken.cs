using TechStore.Domain.Common;
using TechStore.Domain.Enums;

namespace TechStore.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOnUtc { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedOnUtc { get; set; }
    public TokenType TokenType { get; set; } = TokenType.RefreshToken;

    public ApplicationUser? User { get; set; }
}

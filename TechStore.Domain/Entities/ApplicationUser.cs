using TechStore.Domain.Common;
using TechStore.Domain.Enums;

namespace TechStore.Domain.Entities;

public class ApplicationUser : AuditableIdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
    public string? PasswordResetCode { get; set; }
    public DateTime? PasswordResetCodeExpiresOnUtc { get; set; }
    public PasswordResetStatus PasswordResetStatus { get; set; } = PasswordResetStatus.None;

    public Cart? Cart { get; set; }
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

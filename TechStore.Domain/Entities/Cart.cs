using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class Cart : BaseEntity
{
    public int UserId { get; set; }
    public bool IsActive { get; set; } = true;

    public ApplicationUser? User { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

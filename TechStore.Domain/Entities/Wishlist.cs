using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class Wishlist : BaseEntity
{
    public int UserId { get; set; }
    public int ProductId { get; set; }

    public ApplicationUser? User { get; set; }
    public Product? Product { get; set; }
}

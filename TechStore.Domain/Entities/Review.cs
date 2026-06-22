using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class Review : BaseEntity
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int? OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }

    public ApplicationUser? User { get; set; }
    public Product? Product { get; set; }
    public Order? Order { get; set; }
}

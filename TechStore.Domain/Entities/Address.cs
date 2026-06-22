using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class Address : BaseEntity
{
    public int UserId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public ApplicationUser? User { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

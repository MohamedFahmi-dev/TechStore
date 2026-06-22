using TechStore.Domain.Common;

namespace TechStore.Domain.Entities;

public class NewsletterSubscriber : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

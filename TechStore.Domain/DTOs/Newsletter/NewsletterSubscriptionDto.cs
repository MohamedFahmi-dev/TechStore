using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.DTOs.Newsletter;

public class NewsletterSubscriptionDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.DTOs.Auth;

public sealed class ForgotPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}


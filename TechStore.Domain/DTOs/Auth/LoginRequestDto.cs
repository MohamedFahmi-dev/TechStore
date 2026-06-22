using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.DTOs.Auth;

public sealed class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}


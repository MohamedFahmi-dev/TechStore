using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.DTOs.Auth;

public sealed class VerifyResetCodeRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}


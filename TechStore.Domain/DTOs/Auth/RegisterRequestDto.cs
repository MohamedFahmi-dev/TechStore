using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.DTOs.Auth;

public sealed class RegisterRequestDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;
}


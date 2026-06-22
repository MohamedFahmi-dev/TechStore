using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Admin;

public sealed class CreateUserRequestDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public string? PhoneNumber { get; set; }
}


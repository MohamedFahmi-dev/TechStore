using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Admin;

public sealed class UserResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}


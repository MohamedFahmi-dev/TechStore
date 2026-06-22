using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Admin;

public sealed class UpdateUserRequestDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
}


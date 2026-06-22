using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Admin;

public sealed class AssignRoleRequestDto
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}


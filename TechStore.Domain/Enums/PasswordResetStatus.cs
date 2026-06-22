namespace TechStore.Domain.Enums;

public enum PasswordResetStatus
{
    None = 1,
    Pending = 2,
    Verified = 3,
    Expired = 4,
    Used = 5
}

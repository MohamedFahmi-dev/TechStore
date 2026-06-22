namespace TechStore.Domain.DTOs.User;

public sealed class AddAddressRequestDto
{
    public int UserId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}


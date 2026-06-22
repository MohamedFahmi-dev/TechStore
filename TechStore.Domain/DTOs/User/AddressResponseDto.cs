namespace TechStore.Domain.DTOs.User;

public class AddressResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Label { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string Governorate { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsDefault { get; set; }
}

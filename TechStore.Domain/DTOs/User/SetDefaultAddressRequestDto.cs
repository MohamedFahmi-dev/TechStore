namespace TechStore.Domain.DTOs.User;

public sealed class SetDefaultAddressRequestDto
{
    public int UserId { get; set; }
    public int AddressId { get; set; }
}


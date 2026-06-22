namespace TechStore.Domain.DTOs.Order
{
    public class OrderAddressDto
    {
        public string Label { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}

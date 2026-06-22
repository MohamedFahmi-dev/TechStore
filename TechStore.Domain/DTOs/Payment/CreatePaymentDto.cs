namespace TechStore.Domain.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public string Method { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}

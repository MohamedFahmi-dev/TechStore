using TechStore.Domain.DTOs.Payment;

namespace TechStore.BLL.Interfaces;

public interface IStripeService
{
    Task<CreatePaymentIntentResponseDto> CreatePaymentIntentAsync(CreatePaymentIntentDto dto);
    Task<ConfirmPaymentResponseDto> ConfirmPaymentAsync(ConfirmPaymentDto dto);
    Task HandleWebhookAsync(string jsonPayload, string stripeSignature);
    Task<RefundResponseDto> RefundPaymentAsync(string paymentIntentId, decimal? amount = null);
}

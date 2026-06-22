using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using TechStore.BLL.Auth;
using TechStore.BLL.Interfaces;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Payment;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Services;

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StripeService> _logger;


    public StripeService(IOptions<StripeSettings> stripeSettings, IUnitOfWork unitOfWork, ILogger<StripeService> logger)
    {
        _stripeSettings = stripeSettings.Value;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreatePaymentIntentResponseDto> CreatePaymentIntentAsync(CreatePaymentIntentDto dto)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(dto.Amount * 100), // Stripe uses cents
            Currency = dto.Currency.ToLower(),
            PaymentMethodTypes = new List<string> { "card" },
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", dto.OrderId.ToString() },
                { "Description", dto.Description ?? "Order payment" }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return new CreatePaymentIntentResponseDto
        {
            ClientSecret = paymentIntent.ClientSecret,
            PaymentIntentId = paymentIntent.Id,
            Amount = paymentIntent.Amount / 100m,
            Currency = paymentIntent.Currency.ToUpper(),
            Status = paymentIntent.Status
        };
    }

    public async Task<ConfirmPaymentResponseDto> ConfirmPaymentAsync(ConfirmPaymentDto dto)
    {
        var options = new PaymentIntentConfirmOptions
        {
            PaymentMethod = dto.PaymentMethodId
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.ConfirmAsync(dto.PaymentIntentId, options);

        // Update payment status in database
        var payments = await _unitOfWork.Payments.FindAsync(p => p.TransactionId == dto.PaymentIntentId);
        var payment = payments.FirstOrDefault();
        if (payment != null)
        {
            payment.Status = paymentIntent.Status switch
            {
                "succeeded" => PaymentStatus.Paid,
                "requires_payment_method" => PaymentStatus.Pending,
                "requires_confirmation" => PaymentStatus.Pending,
                "requires_action" => PaymentStatus.Pending,
                "canceled" => PaymentStatus.Failed,
                _ => PaymentStatus.Pending
            };

            if (payment.Status == PaymentStatus.Paid)
            {
                payment.PaidAt = DateTime.UtcNow;
            }

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();
        }

        return new ConfirmPaymentResponseDto
        {
            PaymentIntentId = paymentIntent.Id,
            Status = paymentIntent.Status,
            Amount = paymentIntent.Amount / 100m
        };
    }

    public async Task HandleWebhookAsync(string jsonPayload, string stripeSignature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                jsonPayload,
                stripeSignature,
                _stripeSettings.WebhookSecret,
                300 // tolerance in seconds
            );

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                await UpdatePaymentStatusAsync(paymentIntent.Id, PaymentStatus.Paid);
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                await UpdatePaymentStatusAsync(paymentIntent.Id, PaymentStatus.Failed);
            }
            else if (stripeEvent.Type == "payment_intent.canceled")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                await UpdatePaymentStatusAsync(paymentIntent.Id, PaymentStatus.Failed);
            }

        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe webhook error");
            throw;
        }

    }

    private async Task UpdatePaymentStatusAsync(string paymentIntentId, PaymentStatus status)
    {
        var payments = await _unitOfWork.Payments.FindAsync(p => p.TransactionId == paymentIntentId);
        var payment = payments.FirstOrDefault();

        if (payment != null)
        {
            payment.Status = status;

            if (status == PaymentStatus.Paid)
            {
                payment.PaidAt = DateTime.UtcNow;
            }

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            // Update order status if payment is successful
            if (status == PaymentStatus.Paid)
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId);
                if (order != null)
                {
                    order.PaymentStatus = PaymentStatus.Paid;
                    order.OrderStatus = OrderStatus.Processing;
                    _unitOfWork.Orders.Update(order);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }
    }

    public async Task<RefundResponseDto> RefundPaymentAsync(string paymentIntentId, decimal? amount = null)
    {
        var refundOptions = new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId
        };

        if (amount.HasValue)
        {
            refundOptions.Amount = (long)(amount.Value * 100);
        }

        var refundService = new RefundService();
        var refund = await refundService.CreateAsync(refundOptions);

        return new RefundResponseDto
        {
            RefundId = refund.Id,
            PaymentIntentId = refund.PaymentIntentId,
            Amount = refund.Amount / 100m,
            Status = refund.Status,
            Currency = refund.Currency.ToUpper()
        };
    }
}

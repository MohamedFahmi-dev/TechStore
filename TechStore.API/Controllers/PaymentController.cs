using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.API.Filters;
using TechStore.BLL.Interface;
using TechStore.BLL.Interfaces;
using TechStore.Domain.DTOs.Payment;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Payment.Prefix)]
[Authorize]
public class PaymentController : AppControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IStripeService _stripeService;

    public PaymentController(IPaymentService paymentService, IStripeService stripeService)
    {
        _paymentService = paymentService;
        _stripeService = stripeService;
    }

    [HttpPost("create-intent")]
    [Idempotent]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
    {
        var result = await _stripeService.CreatePaymentIntentAsync(dto);
        return Ok(result);
    }

    [HttpPost("confirm")]
    [Idempotent]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDto dto)
    {
        var result = await _stripeService.ConfirmPaymentAsync(dto);
        return Ok(result);
    }

    [HttpPost("webhook")]
    [AllowAnonymous] // Webhooks don't have authentication
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();

        if (string.IsNullOrEmpty(stripeSignature))
        {
            return BadRequest("Stripe-Signature header is missing");
        }

        try
        {
            await _stripeService.HandleWebhookAsync(json, stripeSignature);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("refund/{paymentIntentId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RefundPayment(string paymentIntentId, [FromQuery] decimal? amount = null)
    {
        var result = await _stripeService.RefundPaymentAsync(paymentIntentId, amount);
        return Ok(result);
    }

    [HttpPut(ApiRoutes.Payment.UpdateStatus)]
    [Authorize(Roles = "Admin")] // Payment callbacks usually come from admin or webhooks
    public async Task<IActionResult> UpdateStatus(int orderId, [FromQuery] TechStore.Domain.Enums.PaymentStatus status, [FromQuery] string? transactionId)
    {
        var result = await _paymentService.UpdateStatusAsync(orderId, status, transactionId);
        return Handle(result);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        var result = await _paymentService.GetByOrderIdAsync(orderId);
        return Handle(result);
    }
}

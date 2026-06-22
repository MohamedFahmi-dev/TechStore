using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.Routing;
using TechStore.Domain.DTOs.Newsletter;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Newsletter.Prefix)]
public class NewsletterController : AppControllerBase
{
    private readonly INewsletterService _newsletterService;

    public NewsletterController(INewsletterService newsletterService)
    {
        _newsletterService = newsletterService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _newsletterService.GetAllAsync();
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Newsletter.Subscribe)]
    public async Task<IActionResult> Subscribe([FromBody] NewsletterSubscriptionDto dto)
    {
        var result = await _newsletterService.SubscribeAsync(dto.Email);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Newsletter.Unsubscribe)]
    public async Task<IActionResult> Unsubscribe([FromBody] NewsletterSubscriptionDto dto)
    {
        var result = await _newsletterService.UnsubscribeAsync(dto.Email);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Newsletter.Delete)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _newsletterService.DeleteAsync(id);
        return Handle(result);
    }
}

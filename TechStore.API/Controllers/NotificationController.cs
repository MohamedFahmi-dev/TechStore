using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Notification;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Notification.Prefix)]
[Authorize]
public class NotificationController : AppControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _notificationService.GetByUserAsync(CurrentUserId, page, pageSize);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Notification.UnreadCount)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var result = await _notificationService.GetUnreadCountAsync(CurrentUserId);
        return Handle(result);
    }

    [HttpPut(ApiRoutes.Notification.MarkRead)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _notificationService.MarkAsReadAsync(id, CurrentUserId);
        return Handle(result);
    }

    [HttpPut(ApiRoutes.Notification.MarkAllRead)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _notificationService.MarkAllAsReadAsync(CurrentUserId);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Notification.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _notificationService.DeleteAsync(id, CurrentUserId);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Notification.SendToAll)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendToAll([FromBody] SendNotificationDto dto)
    {
        var result = await _notificationService.SendToAllAsync(dto.Title, dto.Message, dto.Type);
        return Handle(result);
    }
}

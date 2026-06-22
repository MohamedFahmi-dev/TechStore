using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Notification;
using TechStore.Domain.Entities;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<NotificationDto>>> GetByUserAsync(int userId, int page = 1, int pageSize = 20)
    {
        var query = _uow.Notifications.GetTableNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Id);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Result<PagedResult<NotificationDto>>.Ok(new PagedResult<NotificationDto>
        {
            Items = _mapper.Map<List<NotificationDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<Result<int>> GetUnreadCountAsync(int userId)
    {
        var count = await _uow.Notifications.GetTableNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead);
        return Result<int>.Ok(count);
    }

    public async Task<Result> MarkAsReadAsync(int notificationId, int userId)
    {
        var items = await _uow.Notifications.FindAsync(n => n.Id == notificationId && n.UserId == userId);
        var entity = items.FirstOrDefault();

        if (entity is null)
            return Error.NotFound("Notification.NotFound", "Notification not found.");

        entity.IsRead = true;
        entity.ReadAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> MarkAllAsReadAsync(int userId)
    {
        var items = await _uow.Notifications.FindAsync(n => n.UserId == userId && !n.IsRead);
        foreach (var n in items)
        {
            n.IsRead = true;
            n.ReadAt = DateTime.UtcNow;
        }
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int notificationId, int userId)
    {
        var items = await _uow.Notifications.FindAsync(n => n.Id == notificationId && n.UserId == userId);
        var entity = items.FirstOrDefault();

        if (entity is null)
            return Error.NotFound("Notification.NotFound", "Notification not found.");

        _uow.Notifications.Delete(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> SendAsync(int userId, string title, string message, NotificationType type = NotificationType.Info)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false
        };
        await _uow.Notifications.AddAsync(notification);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> SendOrderUpdateAsync(int userId, string orderNumber, OrderStatus status)
    {
        var message = $"Your order #{orderNumber} status has been updated to: {status}.";
        return await SendAsync(userId, "Order Update", message, NotificationType.OrderUpdate);
    }

    public async Task<Result> SendToAllAsync(string title, string message, NotificationType type = NotificationType.Info)
    {
        // TODO: Implement batch notification to all users
        // This would typically use a background service or message queue
        // For now, return success to avoid breaking the API
        await Task.CompletedTask;
        return Result.Ok();
    }
}

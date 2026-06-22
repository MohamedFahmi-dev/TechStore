using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Notification;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Interface;

public interface INotificationService
{
    Task<Result<PagedResult<NotificationDto>>> GetByUserAsync(int userId, int page = 1, int pageSize = 20);
    Task<Result<int>> GetUnreadCountAsync(int userId);
    Task<Result> MarkAsReadAsync(int notificationId, int userId);
    Task<Result> MarkAllAsReadAsync(int userId);
    Task<Result> DeleteAsync(int notificationId, int userId);
    Task<Result> SendAsync(int userId, string title, string message, NotificationType type = NotificationType.Info);
    Task<Result> SendOrderUpdateAsync(int userId, string orderNumber, OrderStatus status);
    Task<Result> SendToAllAsync(string title, string message, NotificationType type = NotificationType.Info);

}


using System.ComponentModel.DataAnnotations;
using TechStore.Domain.Enums;

namespace TechStore.Domain.DTOs.Notification;

public class SendNotificationDto
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Message { get; set; } = null!;

    public NotificationType Type { get; set; } = NotificationType.Info;
}

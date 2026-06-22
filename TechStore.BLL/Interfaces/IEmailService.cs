using TechStore.Domain.Common;

namespace TechStore.BLL.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string content);
}

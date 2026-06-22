using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechStore.BLL.Auth;
using TechStore.BLL.Interfaces;

namespace TechStore.BLL.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string content)
    {
        try
        {
            using var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_settings.FromEmail, _settings.AppPassword);

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, "TechStore"),
                Subject = subject,
                Body = content,
                IsBodyHtml = false
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
            _logger.LogInformation("Email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }
}

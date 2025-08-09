using Microsoft.Extensions.Logging;

namespace InstitutoVirtus.Infrastructure.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendBulkEmailAsync(List<string> recipients, string subject, string body, CancellationToken cancellationToken = default);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implementar envio real de email (SendGrid, SMTP, etc.)
        _logger.LogInformation($"Email enviado para {to}: {subject}");
        return Task.CompletedTask;
    }

    public Task SendBulkEmailAsync(List<string> recipients, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implementar envio em massa
        _logger.LogInformation($"Email em massa enviado para {recipients.Count} destinatários: {subject}");
        return Task.CompletedTask;
    }
}

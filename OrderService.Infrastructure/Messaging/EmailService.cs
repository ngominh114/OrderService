namespace OrderService.Infrastructure.Messaging;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implement email sending logic
        // This could integrate with SMTP, SendGrid, or any other email provider
        await Task.CompletedTask;
    }
}

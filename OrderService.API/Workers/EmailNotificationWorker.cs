namespace OrderService.API.Workers;

using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;

public class EmailNotificationWorker : BackgroundService
{
    private readonly ILogger<EmailNotificationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EmailNotificationWorker(ILogger<EmailNotificationWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Notification Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    // TODO: Implement email notification logic
                    // 1. Fetch pending outbox events for email notifications
                    // 2. Send emails based on event type
                    // 3. Mark events as processed
                    // 4. Handle failures appropriately

                    await Task.Delay(5000, stoppingToken); // Check every 5 seconds
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Email Notification Worker");
            }
        }

        _logger.LogInformation("Email Notification Worker stopped.");
    }
}

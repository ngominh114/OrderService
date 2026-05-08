namespace OrderService.API.Workers;

using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;

public class ProductionSystemWorker : BackgroundService
{
    private readonly ILogger<ProductionSystemWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ProductionSystemWorker(ILogger<ProductionSystemWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Production System Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var notifier = scope.ServiceProvider.GetRequiredService<IProductionSystemNotifier>();

                    // TODO: Implement production system notification logic
                    // 1. Fetch pending outbox events for production notifications
                    // 2. Build production order notifications
                    // 3. Send notifications to production system
                    // 4. Mark events as processed
                    // 5. Handle failures appropriately

                    await Task.Delay(5000, stoppingToken); // Check every 5 seconds
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Production System Worker");
            }
        }

        _logger.LogInformation("Production System Worker stopped.");
    }
}

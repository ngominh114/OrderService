namespace OrderService.API.Workers;

using OrderService.Domain.Constants;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;

public class OutboxWorker : BackgroundService
{
    private readonly ILogger<OutboxWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OutboxWorker(ILogger<OutboxWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mqService = scope.ServiceProvider.GetRequiredService<IMessageQueueService>();

                // Get all pending outbox events
                var pendingEvents = await unitOfWork.OutboxEvents.GetAsync(e => !e.IsProcessed);

                foreach (var evt in pendingEvents)
                {
                    try
                    {
                        await ProcessOutboxEventAsync(evt, unitOfWork, mqService, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing outbox event {EventId}", evt.Id);
                    }
                }

                await Task.Delay(3000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox Worker error");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }

    private async Task ProcessOutboxEventAsync(
        OrderService.Domain.Entities.OutboxEvent evt,
        IUnitOfWork unitOfWork,
        IMessageQueueService mqService,
        CancellationToken stoppingToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(evt.AggregateId);
        if (order == null) return;

        switch (evt.EventType)
        {
            case OutboxEventTypes.PaymentSucceeded: // backward compatibility for older events
                await PublishEmailNotificationAsync(order, mqService, stoppingToken);
                await PublishInvoiceGenerationAsync(order, mqService, stoppingToken);
                await PublishProductionOrderAsync(order, mqService, stoppingToken);
                break;
            case OutboxEventTypes.EmailNotificationRequested:
                await PublishEmailNotificationAsync(order, mqService, stoppingToken);
                break;
            case OutboxEventTypes.InvoiceGenerationRequested:
                await PublishInvoiceGenerationAsync(order, mqService, stoppingToken);
                break;
            case OutboxEventTypes.ProductionOrderRequested:
                await PublishProductionOrderAsync(order, mqService, stoppingToken);
                break;
        }

        evt.IsProcessed = true;
        evt.ProcessedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(stoppingToken);
        _logger.LogInformation("Outbox event {EventId} processed", evt.Id);
    }

    private async Task PublishEmailNotificationAsync(
        OrderService.Domain.Entities.Order order,
        IMessageQueueService mqService,
        CancellationToken stoppingToken)
    {
        var message = new EmailNotificationMessage
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            Name = order.DisplayName,
            Amount = order.Payment?.Amount.Amount ?? 0,
            Currency = order.Payment?.Amount.Currency ?? "USD"
        };

        await mqService.PublishAsync("email-notifications", message, stoppingToken);
        _logger.LogInformation("Email notification queued for order {OrderId}", order.Id);
    }

    private async Task PublishInvoiceGenerationAsync(
        OrderService.Domain.Entities.Order order,
        IMessageQueueService mqService,
        CancellationToken stoppingToken)
    {
        var message = new InvoiceGenerationMessage
        {
            OrderId = order.Id,
            Name = order.DisplayName,
            Amount = order.Payment?.Amount.Amount ?? 0,
            Currency = order.Payment?.Amount.Currency ?? "USD"
        };

        await mqService.PublishAsync("invoice-generation", message, stoppingToken);
        _logger.LogInformation("Invoice generation queued for order {OrderId}", order.Id);
    }

    private async Task PublishProductionOrderAsync(
        OrderService.Domain.Entities.Order order,
        IMessageQueueService mqService,
        CancellationToken stoppingToken)
    {
        var message = new ProductionOrderMessage
        {
            OrderId = order.Id,
            Name = order.DisplayName,
            CustomerId = order.CustomerId,
            Amount = order.Payment?.Amount.Amount ?? 0,
            Currency = order.Payment?.Amount.Currency ?? "USD",
            Items = new List<ProductionItemMessage>()
        };

        await mqService.PublishAsync("production-orders", message, stoppingToken);
        _logger.LogInformation("Production order queued for {OrderId}", order.Id);
    }
}

public class EmailNotificationMessage
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class InvoiceGenerationMessage
{
    public Guid OrderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class ProductionOrderMessage
{
    public Guid OrderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public List<ProductionItemMessage> Items { get; set; } = [];
}

public class ProductionItemMessage
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

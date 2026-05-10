namespace OrderService.API.Workers;

using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;

public class OutboxWorker : BackgroundService
{
    private const int BatchSize = 50;
    private const string OrderEventsTopicName = "order-events";
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(3);

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

                var pendingEvents = (await unitOfWork.OutboxEvents.GetAsync(e => !e.IsProcessed))
                    .OrderBy(e => e.CreatedAt)
                    .Take(BatchSize)
                    .ToList();

                foreach (var outboxEvent in pendingEvents)
                {
                    try
                    {
                        await ProcessOutboxEventAsync(outboxEvent, unitOfWork, mqService, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing outbox event {EventId}", outboxEvent.Id);
                    }
                }

                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox Worker error");
                await Task.Delay(PollInterval, stoppingToken);
            }
        }
    }

    private async Task ProcessOutboxEventAsync(
        OutboxEvent outboxEvent,
        IUnitOfWork unitOfWork,
        IMessageQueueService mqService,
        CancellationToken stoppingToken)
    {
        var outboxEnvelope = new
        {
            MessageId = outboxEvent.Id,
            AggregateId = outboxEvent.AggregateId,
            EventType = outboxEvent.EventType,
            Payload = outboxEvent.Payload,
            OccurredAt = outboxEvent.CreatedAt
        };

        await mqService.PublishAsync(OrderEventsTopicName, outboxEnvelope, stoppingToken);
        _logger.LogInformation(
            "Outbox event {EventId} published to topic {TopicName}",
            outboxEvent.Id,
            OrderEventsTopicName);

        // Mark processed only after publish succeeds.
        outboxEvent.IsProcessed = true;
        outboxEvent.ProcessedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(stoppingToken);
        _logger.LogInformation("Outbox event {EventId} processed", outboxEvent.Id);
    }
}

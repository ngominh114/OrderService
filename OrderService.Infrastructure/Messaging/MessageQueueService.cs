namespace OrderService.Infrastructure.Messaging;

using System.Text.Json;
using Microsoft.Extensions.Logging;

public class MessageQueueService : IMessageQueueService
{
    private readonly ILogger<MessageQueueService> _logger;

    // Mock SQS queues - replace with real AWS SDK in production
    private static readonly Dictionary<string, Queue<string>> MockQueues = new()
    {
        { "email-notifications", new Queue<string>() },
        { "invoice-generation", new Queue<string>() },
        { "production-orders", new Queue<string>() }
    };

    public MessageQueueService(ILogger<MessageQueueService> logger) => _logger = logger;

    public async Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var payload = JsonSerializer.Serialize(message);
            if (MockQueues.TryGetValue(queueName, out var queue))
            {
                queue.Enqueue(payload);
                _logger.LogInformation("Published to {Queue}: {Type}", queueName, typeof(T).Name);
            }
            else
            {
                _logger.LogWarning("Queue not found: {Queue}", queueName);
            }
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish to {Queue}", queueName);
            throw;
        }
    }

    public static Queue<string>? GetMockQueue(string queueName) =>
        MockQueues.TryGetValue(queueName, out var q) ? q : null;
}


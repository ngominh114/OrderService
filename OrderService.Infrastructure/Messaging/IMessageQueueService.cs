namespace OrderService.Infrastructure.Messaging;

public interface IMessageQueueService
{
    Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class;
}

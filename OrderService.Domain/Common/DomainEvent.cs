namespace OrderService.Domain.Common;

public abstract class DomainEvent
{
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

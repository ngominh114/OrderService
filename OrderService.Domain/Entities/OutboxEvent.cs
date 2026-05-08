namespace OrderService.Domain.Entities;

using OrderService.Domain.Common;

public class OutboxEvent : BaseEntity
{
    public string AggregateId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

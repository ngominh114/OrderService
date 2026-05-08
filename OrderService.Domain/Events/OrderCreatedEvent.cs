namespace OrderService.Domain.Events;

using OrderService.Domain.Common;

public class OrderCreatedEvent : DomainEvent
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

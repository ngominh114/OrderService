namespace OrderService.Domain.Entities;

using OrderService.Domain.Common;
using OrderService.Domain.Enums;

public class Order : BaseEntity, IAggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = new();

    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public Payment? Payment { get; set; }
    public Invoice? Invoice { get; set; }

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

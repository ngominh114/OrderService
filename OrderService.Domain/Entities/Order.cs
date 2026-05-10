namespace OrderService.Domain.Entities;

using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = string.Empty;
    public Money Cost { get; set; } = new(0);
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CheckedOutAt { get; set; }
    public int[] ImageIds { get; set; } = [];
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Customer? Customer { get; set; }
    public Payment? Payment { get; set; }
    public Invoice? Invoice { get; set; }
}

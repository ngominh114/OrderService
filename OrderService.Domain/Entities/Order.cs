namespace OrderService.Domain.Entities;

using OrderService.Domain.Enums;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CheckedOutAt { get; set; }
    public int[] ImageIds { get; set; } = [];

    public Customer? Customer { get; set; }
    public Payment? Payment { get; set; }
    public Invoice? Invoice { get; set; }
}

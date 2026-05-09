namespace OrderService.Domain.Entities;

using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Money Amount { get; set; } = new(0);
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public Order Order { get; set; } = null!;
}

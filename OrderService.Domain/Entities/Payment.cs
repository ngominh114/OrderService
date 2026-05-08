namespace OrderService.Domain.Entities;

using OrderService.Domain.Enums;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public Order Order { get; set; } = null!;
}

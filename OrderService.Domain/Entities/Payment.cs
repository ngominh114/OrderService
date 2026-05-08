namespace OrderService.Domain.Entities;

using OrderService.Domain.Common;
using OrderService.Domain.Enums;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
    public DateTime? ProcessedAt { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
}

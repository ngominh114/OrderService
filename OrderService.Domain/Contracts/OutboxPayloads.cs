namespace OrderService.Domain.Contracts;

public sealed class ProductionOrderRequestedPayload
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
}

public sealed class InvoiceGenerationRequestedPayload
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public sealed class EmailNotificationRequestedPayload
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

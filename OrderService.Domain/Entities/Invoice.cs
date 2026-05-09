namespace OrderService.Domain.Entities;

using OrderService.Domain.ValueObjects;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Money Amount { get; set; } = new(0);
    public DateTime IssuedAt { get; set; }
    public string? FilePath { get; set; }
    public Order Order { get; set; } = null!;
}

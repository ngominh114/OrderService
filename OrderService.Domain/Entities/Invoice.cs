namespace OrderService.Domain.Entities;

using OrderService.Domain.Common;

public class Invoice : BaseEntity
{
    public int OrderId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
    public string? FilePath { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
}

namespace OrderService.Domain.Entities;

using OrderService.Domain.Common;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
}

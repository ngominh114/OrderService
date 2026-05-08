namespace OrderService.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CheckedOutAt { get; set; }
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

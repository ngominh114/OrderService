namespace OrderService.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CheckedOutAt { get; set; }
}

namespace OrderService.Application.DTOs;

using OrderService.Domain.ValueObjects;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Money Amount { get; set; } = new(0);
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}

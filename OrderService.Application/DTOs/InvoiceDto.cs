namespace OrderService.Application.DTOs;

using OrderService.Domain.ValueObjects;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Money Amount { get; set; } = new(0);
    public DateTime IssuedAt { get; set; }
}

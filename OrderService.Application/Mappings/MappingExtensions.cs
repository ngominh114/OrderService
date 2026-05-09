namespace OrderService.Application.Mappings;

using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

public static class MappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            DisplayName = order.DisplayName,
            Price = order.Price,
            Status = order.Status.ToString(),
            CustomerId = order.CustomerId,
            CreatedAt = order.CreatedAt,
            CheckedOutAt = order.CheckedOutAt
        };
    }

    public static PaymentDto ToDto(this Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status.ToString(),
            PaymentMethod = payment.PaymentMethod,
            TransactionId = payment.TransactionId
        };
    }

    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            OrderId = invoice.OrderId,
            InvoiceNumber = invoice.InvoiceNumber,
            Amount = invoice.Amount,
            IssuedAt = invoice.IssuedAt
        };
    }
}

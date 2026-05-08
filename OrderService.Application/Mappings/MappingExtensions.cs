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
            OrderNumber = order.OrderNumber,
            TotalAmount = order.TotalAmount,
            Status = (int)order.Status,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(x => new OrderItemDto
            {
                Id = x.Id,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                TotalPrice = x.TotalPrice
            }).ToList()
        };
    }

    public static PaymentDto ToDto(this Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = (int)payment.Status,
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
            IssuedDate = invoice.IssuedDate
        };
    }
}

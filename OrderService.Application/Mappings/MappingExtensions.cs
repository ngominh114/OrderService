namespace OrderService.Application.Mappings;

using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

public static class MappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            DisplayName = order.DisplayName,
            Price = order.Cost.Amount,
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

    public static Payment ToPaymentEntity(
        this PaymentProcessingResult result,
        Guid orderId,
        string paymentMethod,
        string idempotencyKey,
        decimal amount,
        string currency)
    {
        return new Payment
        {
            OrderId = orderId,
            Amount = new(amount, currency),
            PaymentMethod = paymentMethod,
            IdempotencyKey = idempotencyKey,
            TransactionId = result.TransactionId,
            Status = result.IsSuccess ? PaymentStatus.Succeeded : PaymentStatus.Failed,
            FailureReason = result.FailureReason,
            ProcessedAt = DateTime.UtcNow
        };
    }
}

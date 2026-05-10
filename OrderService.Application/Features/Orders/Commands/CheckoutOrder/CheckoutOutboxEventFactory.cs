namespace OrderService.Application.Features.Orders.Commands.CheckoutOrder;

using System.Text.Json;
using OrderService.Domain.Constants;
using OrderService.Domain.Contracts;
using OrderService.Domain.Entities;

public static class CheckoutOutboxEventFactory
{
    public static OutboxEvent CreateProductionOrderRequested(Order order, string transactionId)
    {
        var payload = JsonSerializer.Serialize(new ProductionOrderRequestedPayload
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TransactionId = transactionId
        });

        return new OutboxEvent
        {
            AggregateId = order.Id,
            EventType = OutboxEventTypes.ProductionOrderRequested,
            Payload = payload
        };
    }

    public static OutboxEvent CreateInvoiceGenerationRequested(Order order)
    {
        var payload = JsonSerializer.Serialize(new InvoiceGenerationRequestedPayload
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            Amount = order.Cost.Amount,
            Currency = order.Cost.Currency
        });

        return new OutboxEvent
        {
            AggregateId = order.Id,
            EventType = OutboxEventTypes.InvoiceGenerationRequested,
            Payload = payload
        };
    }

    public static OutboxEvent CreateEmailNotificationRequested(Order order)
    {
        var payload = JsonSerializer.Serialize(new EmailNotificationRequestedPayload
        {
            OrderId = order.Id,
            Amount = order.Cost.Amount,
            Currency = order.Cost.Currency
        });

        return new OutboxEvent
        {
            AggregateId = order.Id,
            EventType = OutboxEventTypes.EmailNotificationRequested,
            Payload = payload
        };
    }
}

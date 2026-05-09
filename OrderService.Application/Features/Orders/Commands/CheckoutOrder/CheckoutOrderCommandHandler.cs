namespace OrderService.Application.Features.Orders.Commands.CheckoutOrder;

using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, CheckoutOrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CheckoutOrderResponse> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException("Order not found");

        if (order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("Order cannot be checked out");

        var transactionId = Guid.NewGuid().ToString();

        // Simulate payment (80% success)
        var isSuccess = new Random().Next(100) < 80;
        if (!isSuccess)
            throw new InvalidOperationException("Payment declined");

        // Create payment
        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.Cost,
            PaymentMethod = request.PaymentMethod,
            TransactionId = transactionId,
            Status = PaymentStatus.Succeeded,
            ProcessedAt = DateTime.UtcNow
        };

        order.Payment = payment;
        order.Status = OrderStatus.Processing;
        order.CheckedOutAt = DateTime.UtcNow;

        // Create outbox event for background worker to process
        var outboxEvent = new OutboxEvent
        {
            AggregateId = order.Id,
            EventType = "PaymentSucceeded",
            Payload = $"{{\"OrderId\":\"{order.Id}\",\"Amount\":{order.Cost.Amount}}}"
        };

        await _unitOfWork.OutboxEvents.AddAsync(outboxEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CheckoutOrderResponse
        {
            OrderId = request.OrderId,
            TransactionId = transactionId,
            Message = "Payment successful"
        };
    }
}

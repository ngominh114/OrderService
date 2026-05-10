namespace OrderService.Application.Features.Orders.Commands.CheckoutOrder;

using MediatR;
using OrderService.Application.Interfaces;
using OrderService.Application.Mappings;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, CheckoutOrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentProcessorFactory _paymentProcessorFactory;

    public CheckoutOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IPaymentProcessorFactory paymentProcessorFactory)
    {
        _unitOfWork = unitOfWork;
        _paymentProcessorFactory = paymentProcessorFactory;
    }

    public async Task<CheckoutOrderResponse> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate order exists AND belongs to the requesting customer (single query)
        var order = await _unitOfWork.Orders.GetByIdAndCustomerIdAsync(
            request.CustomerId,
            request.OrderId,
            cancellationToken);

        if (order == null)
            throw new InvalidOperationException("Order not found");

        if (order.Payment?.IdempotencyKey == request.IdempotencyKey)
        {
            return new CheckoutOrderResponse
            {
                OrderId = request.OrderId,
                TransactionId = order.Payment.TransactionId,
                Message = order.Payment.Status == PaymentStatus.Succeeded
                    ? "Payment already processed"
                    : "Payment request already processed"
            };
        }

        // - Draft/PaymentFailed: can start a payment attempt
        // - PaymentPending/Processing/Completed/Cancelled: block new attempts
        var canAttemptPayment =
            order.Status == OrderStatus.Draft ||
            order.Status == OrderStatus.PaymentFailed;
        if (!canAttemptPayment)
            throw new InvalidOperationException("Order is not eligible for payment");

        if (order.Payment?.Status == PaymentStatus.Succeeded)
            throw new InvalidOperationException("Order has already been paid successfully");

        var paymentProcessor = _paymentProcessorFactory.GetProcessor(request.PaymentMethod);

        // Mark order as payment in-progress with atomic transition to avoid concurrent checkouts
        var transitioned = await _unitOfWork.Orders.TryTransitionToPaymentPendingAsync(
            request.CustomerId,
            request.OrderId,
            cancellationToken);

        if (!transitioned)
            throw new InvalidOperationException("Order is currently being checked out");

        order.Status = OrderStatus.PaymentPending;

        PaymentProcessingResult paymentResult;
        OutboxEvent? productionOutbox = null;
        OutboxEvent? invoiceOutbox = null;
        OutboxEvent? emailOutbox = null;
        try
        {
            paymentResult = await paymentProcessor.ProcessAsync(
                new PaymentProcessingRequest
                {
                    OrderId = order.Id,
                    CustomerId = request.CustomerId,
                    Amount = order.Cost.Amount,
                    Currency = order.Cost.Currency,
                    PaymentMethod = request.PaymentMethod
                },
                cancellationToken);
        }
        catch (Exception)
        {
            order.Status = OrderStatus.PaymentFailed;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Payment provider call failed");
        }

        try
        {
            var payment = paymentResult.ToPaymentEntity(
                order.Id,
                request.PaymentMethod,
                request.IdempotencyKey,
                order.Cost.Amount,
                order.Cost.Currency);

            if (order.Payment == null)
            {
                order.Payment = payment;
            }
            else
            {
                // One-to-one Payment row already exists -> update latest attempt result
                order.Payment.PaymentMethod = payment.PaymentMethod;
                order.Payment.TransactionId = payment.TransactionId;
                order.Payment.Status = payment.Status;
                order.Payment.FailureReason = payment.FailureReason;
                order.Payment.ProcessedAt = payment.ProcessedAt;
                order.Payment.Amount = payment.Amount;
                order.Payment.IdempotencyKey = request.IdempotencyKey;
            }

            if (!paymentResult.IsSuccess)
            {
                order.Status = OrderStatus.PaymentFailed;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new InvalidOperationException(
                    paymentResult.FailureReason ?? "Payment failed");
            }

            order.Status = OrderStatus.Processing;
            order.CheckedOutAt = DateTime.UtcNow;

            productionOutbox = CheckoutOutboxEventFactory.CreateProductionOrderRequested(order, paymentResult.TransactionId);
            invoiceOutbox = CheckoutOutboxEventFactory.CreateInvoiceGenerationRequested(order);
            emailOutbox = CheckoutOutboxEventFactory.CreateEmailNotificationRequested(order);

            await _unitOfWork.OutboxEvents.AddAsync(productionOutbox, cancellationToken);

            await _unitOfWork.OutboxEvents.AddAsync(invoiceOutbox, cancellationToken);

            await _unitOfWork.OutboxEvents.AddAsync(emailOutbox, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CheckoutOrderResponse
            {
                OrderId = request.OrderId,
                TransactionId = paymentResult.TransactionId,
                Message = "Payment successful"
            };
        }
        catch (Exception ex) when (paymentResult.IsSuccess)
        {
            // Can be done also with outbox pattern, worker who receiving compensate message will retry handling it until compensation succeeds, but for simplicity we do it here directly.
            var compensation = await paymentProcessor.CompensateAsync(
                new PaymentCompensationRequest
                {
                    OrderId = order.Id,
                    TransactionId = paymentResult.TransactionId,
                    Amount = order.Cost.Amount,
                    Currency = order.Cost.Currency,
                    Reason = "Internal failure after successful payment"
                },
                cancellationToken);

            order.Status = OrderStatus.PaymentFailed;

            // Prevent accidental persistence of pending outbox events if SaveChanges failed earlier.
            if (productionOutbox != null)
                await _unitOfWork.OutboxEvents.DeleteAsync(productionOutbox, cancellationToken);

            if (invoiceOutbox != null)
                await _unitOfWork.OutboxEvents.DeleteAsync(invoiceOutbox, cancellationToken);

            if (emailOutbox != null)
                await _unitOfWork.OutboxEvents.DeleteAsync(emailOutbox, cancellationToken);

            if (order.Payment != null)
            {
                order.Payment.Status = compensation.IsSuccess ? PaymentStatus.Refunded : PaymentStatus.Failed;
                order.Payment.FailureReason = compensation.IsSuccess
                    ? "Payment compensated due to internal failure"
                    : compensation.FailureReason ?? "Compensation failed after internal error";
                order.Payment.ProcessedAt = DateTime.UtcNow;
            }

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
            }

            if (compensation.IsSuccess)
            {
                throw new InvalidOperationException(
                    "Checkout failed due to internal error. Payment has been compensated.",
                    ex);
            }

            throw new InvalidOperationException(
                "Checkout failed due to internal error and payment compensation failed.",
                ex);
        }
    }
}

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
        var order = await LoadOrderOrThrowAsync(request.CustomerId, request.OrderId, cancellationToken);

        var idempotentResponse = GetIdempotentResponse(order, request);
        if (idempotentResponse != null)
            return idempotentResponse;

        ValidateCheckoutEligibility(order);

        IPaymentProcessor paymentProcessor;
        try
        {
            paymentProcessor = _paymentProcessorFactory.GetProcessor(request.PaymentMethod);
        }
        catch (NotSupportedException ex)
        {
            throw new InvalidOperationException("Unsupported payment method", ex);
        }

        await _unitOfWork.ExecuteTransactionAsync(async ct =>
        {
            order = await LoadOrderOrThrowAsync(request.CustomerId, request.OrderId, ct);

            ValidateCheckoutEligibilityForTransition(order);

            order.Status = OrderStatus.PaymentPending;

            if (order.Payment == null)
            {
                order.Payment = new Payment
                {
                    OrderId = order.Id,
                    Amount = order.Cost,
                    PaymentMethod = request.PaymentMethod,
                    IdempotencyKey = request.IdempotencyKey,
                    TransactionId = string.Empty,
                    Status = PaymentStatus.Pending
                };
            }
            else
            {
                order.Payment.PaymentMethod = request.PaymentMethod;
                order.Payment.IdempotencyKey = request.IdempotencyKey;
                order.Payment.Status = PaymentStatus.Pending;
                order.Payment.FailureReason = null;
                order.Payment.ProcessedAt = null;
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);

        PaymentProcessingResult paymentResult;
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
        catch (Exception ex) when (ex is TimeoutException or IOException or HttpRequestException or TaskCanceledException)
        {
            await _unitOfWork.ExecuteTransactionAsync(async ct =>
            {
                order = await LoadOrderOrThrowAsync(request.CustomerId, request.OrderId, ct);

                order.Status = OrderStatus.PaymentVerificationPending;

                if (order.Payment != null)
                {
                    order.Payment.Status = PaymentStatus.VerificationPending;
                    order.Payment.FailureReason = "Payment result unknown due to provider timeout/network failure";
                    order.Payment.ProcessedAt = DateTime.UtcNow;
                }

                await _unitOfWork.SaveChangesAsync(ct);
            }, cancellationToken);

            throw new InvalidOperationException(
                "Payment result is unknown. Order moved to PaymentVerificationPending for reconciliation.",
                ex);
        }

        var payment = paymentResult.ToPaymentEntity(
            order.Id,
            request.PaymentMethod,
            request.IdempotencyKey,
            order.Cost.Amount,
            order.Cost.Currency);

        if (!paymentResult.IsSuccess)
        {
            await _unitOfWork.ExecuteTransactionAsync(async ct =>
            {
                order = await LoadOrderOrThrowAsync(request.CustomerId, request.OrderId, ct);

                UpsertPayment(order, payment, request.IdempotencyKey);
                order.Status = OrderStatus.PaymentFailed;
                await _unitOfWork.SaveChangesAsync(ct);
            }, cancellationToken);

            throw new InvalidOperationException(paymentResult.FailureReason ?? "Payment failed");
        }

        try
        {
            await _unitOfWork.ExecuteTransactionAsync(async ct =>
            {
                order = await LoadOrderOrThrowAsync(request.CustomerId, request.OrderId, ct);

                UpsertPayment(order, payment, request.IdempotencyKey);
                order.Status = OrderStatus.Processing;
                order.CheckedOutAt = DateTime.UtcNow;

                var productionOutbox = CheckoutOutboxEventFactory.CreateProductionOrderRequested(order, paymentResult.TransactionId);
                var invoiceOutbox = CheckoutOutboxEventFactory.CreateInvoiceGenerationRequested(order);
                var emailOutbox = CheckoutOutboxEventFactory.CreateEmailNotificationRequested(order);

                await _unitOfWork.OutboxEvents.AddAsync(productionOutbox, ct);
                await _unitOfWork.OutboxEvents.AddAsync(invoiceOutbox, ct);
                await _unitOfWork.OutboxEvents.AddAsync(emailOutbox, ct);

                await _unitOfWork.SaveChangesAsync(ct);
            }, cancellationToken);

            return new CheckoutOrderResponse
            {
                OrderId = request.OrderId,
                TransactionId = paymentResult.TransactionId,
                Message = "Payment successful"
            };
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
        {
            throw new InvalidOperationException("Order is being checked out concurrently. Please retry.", ex);
        }
        catch (Exception ex)
        {
            // Compensatation should be done with outbox pattern to make sure worker can retry in case of transient failure. We implement it directly here for simplicity.
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

            await _unitOfWork.ExecuteTransactionAsync(async ct =>
            {
                order = await LoadOrderOrThrowAsync(request.CustomerId, request.OrderId, ct);

                if (order.Payment != null)
                {
                    order.Payment.Status = compensation.IsSuccess ? PaymentStatus.Refunded : PaymentStatus.Failed;
                    order.Payment.FailureReason = compensation.IsSuccess
                        ? "Payment compensated due to internal failure"
                        : compensation.FailureReason ?? "Compensation failed after internal error";
                    order.Payment.ProcessedAt = DateTime.UtcNow;
                }

                order.Status = OrderStatus.PaymentFailed;
                await _unitOfWork.SaveChangesAsync(ct);
            }, cancellationToken);

            throw new InvalidOperationException(
                compensation.IsSuccess
                    ? "Checkout failed due to internal error. Payment has been compensated."
                    : "Checkout failed due to internal error and payment compensation failed.",
                ex);
        }
    }

    private static void UpsertPayment(Order order, Payment payment, string idempotencyKey)
    {
        if (order.Payment == null)
        {
            order.Payment = payment;
            return;
        }

        // One-to-one Payment row already exists -> update latest attempt result
        order.Payment.PaymentMethod = payment.PaymentMethod;
        order.Payment.TransactionId = payment.TransactionId;
        order.Payment.Status = payment.Status;
        order.Payment.FailureReason = payment.FailureReason;
        order.Payment.ProcessedAt = payment.ProcessedAt;
        order.Payment.Amount = payment.Amount;
        order.Payment.IdempotencyKey = idempotencyKey;
    }

    private static CheckoutOrderResponse? GetIdempotentResponse(Order order, CheckoutOrderCommand request)
    {
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

        return null;
    }

    private static void ValidateCheckoutEligibility(Order order)
    {
        var canAttemptPayment = order.Status == OrderStatus.Draft || order.Status == OrderStatus.PaymentFailed;
        if (!canAttemptPayment)
            throw new InvalidOperationException("Order is not eligible for payment");

        if (order.Payment?.Status == PaymentStatus.Succeeded)
            throw new InvalidOperationException("Order has already been paid successfully");
    }

    private static void ValidateCheckoutEligibilityForTransition(Order order)
    {
        ValidateCheckoutEligibility(order);
    }

    private async Task<Order> LoadOrderOrThrowAsync(Guid customerId, Guid orderId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Orders.GetByIdAndCustomerIdAsync(customerId, orderId, cancellationToken)
            ?? throw new InvalidOperationException("Order not found");
    }
}

namespace OrderService.Application.Interfaces;

public interface IPaymentProcessor
{
    Task<PaymentProcessingResult> ProcessAsync(
        PaymentProcessingRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentCompensationResult> CompensateAsync(
        PaymentCompensationRequest request,
        CancellationToken cancellationToken = default);
}

public interface INamedPaymentProcessor : IPaymentProcessor
{
    IReadOnlyCollection<string> SupportedMethods { get; }
}

public sealed class PaymentProcessingRequest
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string PaymentMethod { get; set; } = string.Empty;
}

public sealed class PaymentProcessingResult
{
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
}

public sealed class PaymentCompensationRequest
{
    public Guid OrderId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Reason { get; set; } = string.Empty;
}

public sealed class PaymentCompensationResult
{
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
}

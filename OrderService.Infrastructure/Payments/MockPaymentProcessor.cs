namespace OrderService.Infrastructure.Payments;

using OrderService.Application.Interfaces;

public class MockPaymentProcessor : INamedPaymentProcessor
{
    private const int PaymentSuccessRatePercent = 80;
    private const int CompensationSuccessRatePercent = 100;

    public IReadOnlyCollection<string> SupportedMethods { get; } = ["card", "bank_transfer"];

    public Task<PaymentProcessingResult> ProcessAsync(
        PaymentProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        var success = IsSuccess(PaymentSuccessRatePercent);

        var result = new PaymentProcessingResult
        {
            IsSuccess = success,
            TransactionId = Guid.NewGuid().ToString(),
            FailureReason = success ? null : "Payment declined by provider"
        };

        return Task.FromResult(result);
    }

    public Task<PaymentCompensationResult> CompensateAsync(
        PaymentCompensationRequest request,
        CancellationToken cancellationToken = default)
    {
        var success = IsSuccess(CompensationSuccessRatePercent);

        return Task.FromResult(new PaymentCompensationResult
        {
            IsSuccess = success,
            FailureReason = success ? null : "Compensation request failed at provider"
        });
    }

    private static bool IsSuccess(int successRatePercent)
    {
        var normalizedRate = Math.Clamp(successRatePercent, 0, 100);
        return Random.Shared.Next(100) < normalizedRate;
    }
}

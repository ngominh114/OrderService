namespace OrderService.Infrastructure.Payments;

using OrderService.Application.Interfaces;

public class MomoPaymentProcessor : INamedPaymentProcessor
{
    public IReadOnlyCollection<string> SupportedMethods { get; } = ["momo"];

    public Task<PaymentProcessingResult> ProcessAsync(
        PaymentProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Momo payment processor is not implemented yet.");
    }

    public Task<PaymentCompensationResult> CompensateAsync(
        PaymentCompensationRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Momo compensation is not implemented yet.");
    }
}

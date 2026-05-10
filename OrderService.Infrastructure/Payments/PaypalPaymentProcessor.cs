namespace OrderService.Infrastructure.Payments;

using OrderService.Application.Interfaces;

public class PaypalPaymentProcessor : INamedPaymentProcessor
{
    public IReadOnlyCollection<string> SupportedMethods { get; } = ["paypal"];

    public Task<PaymentProcessingResult> ProcessAsync(
        PaymentProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("PayPal payment processor is not implemented yet.");
    }

    public Task<PaymentCompensationResult> CompensateAsync(
        PaymentCompensationRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("PayPal compensation is not implemented yet.");
    }
}

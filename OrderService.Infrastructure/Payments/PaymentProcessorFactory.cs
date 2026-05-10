namespace OrderService.Infrastructure.Payments;

using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;

public class PaymentProcessorFactory : IPaymentProcessorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentProcessor GetProcessor(string paymentMethod)
    {
        return paymentMethod?.Trim().ToLowerInvariant() switch
        {
            "card" => _serviceProvider.GetRequiredService<MockPaymentProcessor>(),
            "bank_transfer" => _serviceProvider.GetRequiredService<MockPaymentProcessor>(),
            "paypal" => _serviceProvider.GetRequiredService<PaypalPaymentProcessor>(),
            "momo" => _serviceProvider.GetRequiredService<MomoPaymentProcessor>(),
            _ => throw new NotSupportedException($"Payment method '{paymentMethod}' is not supported.")
        };
    }
}

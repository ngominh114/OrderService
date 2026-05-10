namespace OrderService.Application.Interfaces;

public interface IPaymentProcessorFactory
{
    IPaymentProcessor GetProcessor(string paymentMethod);
}

namespace OrderService.Infrastructure.Messaging;

public interface IPaymentProvider
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default);
}

public class PaymentRequest
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
}

public class PaymentResult
{
    public bool IsSuccessful { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

namespace OrderService.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderResponse
{
    public Guid OrderId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

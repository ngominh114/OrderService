namespace OrderService.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderResponse
{
    public int OrderId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
}

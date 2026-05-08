namespace OrderService.Application.Features.Orders.Commands.CheckoutOrder;

using MediatR;

public class CheckoutOrderCommand : IRequest<CheckoutOrderResponse>
{
    public Guid OrderId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

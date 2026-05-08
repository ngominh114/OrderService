namespace OrderService.Application.Features.Orders.Commands.CheckoutOrder;

using MediatR;
using OrderService.Application.Common;

public class CheckoutOrderCommand : IRequest<Result<CheckoutOrderResponse>>
{
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

namespace OrderService.Application.Features.Orders.Commands.CheckoutOrder;

using System.ComponentModel.DataAnnotations;
using MediatR;

public class CheckoutOrderCommand : IRequest<CheckoutOrderResponse>
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string IdempotencyKey { get; set; } = string.Empty;
}

namespace OrderService.Domain.Events;

using OrderService.Domain.Common;

public class PaymentSucceededEvent : DomainEvent
{
    public int OrderId { get; set; }
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
}

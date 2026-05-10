namespace OrderService.Domain.Enums;

public enum OrderStatus
{
    Draft = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    PaymentPending = 6,
    PaymentFailed = 7
}

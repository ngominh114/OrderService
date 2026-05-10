namespace OrderService.Domain.Constants;

public static class OutboxEventTypes
{
    public const string PaymentSucceeded = "PaymentSucceeded";
    public const string ProductionOrderRequested = "ProductionOrderRequested";
    public const string InvoiceGenerationRequested = "InvoiceGenerationRequested";
    public const string EmailNotificationRequested = "EmailNotificationRequested";
}

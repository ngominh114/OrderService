namespace OrderService.Infrastructure.Messaging;

public interface IProductionSystemNotifier
{
    Task NotifyProductionSystemAsync(ProductionNotification notification, CancellationToken cancellationToken = default);
}

public class ProductionNotification
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public List<ProductionItem> Items { get; set; } = new();
}

public class ProductionItem
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class ProductionSystemNotifier : IProductionSystemNotifier
{
    public async Task NotifyProductionSystemAsync(ProductionNotification notification, CancellationToken cancellationToken = default)
    {
        // TODO: Implement production system notification logic
        // This could be an HTTP call, message queue, or any other integration
        await Task.CompletedTask;
    }
}

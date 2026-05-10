namespace OrderService.Domain.Interfaces;

using OrderService.Domain.Entities;

public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    IRepository<Payment> Payments { get; }
    IRepository<Invoice> Invoices { get; }
    IRepository<OutboxEvent> OutboxEvents { get; }

    Task ExecuteTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

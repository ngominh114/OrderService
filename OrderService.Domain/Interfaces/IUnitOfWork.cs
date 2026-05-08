namespace OrderService.Domain.Interfaces;

using OrderService.Domain.Entities;

public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    IRepository<Payment> Payments { get; }
    IRepository<Invoice> Invoices { get; }
    IRepository<OutboxEvent> OutboxEvents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

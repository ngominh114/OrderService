namespace OrderService.Infrastructure.Persistence;

using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IOrderRepository? _orderRepository;
    private IRepository<Payment>? _paymentRepository;
    private IRepository<Invoice>? _invoiceRepository;
    private IRepository<OutboxEvent>? _outboxEventRepository;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IOrderRepository Orders 
        => _orderRepository ??= new OrderRepository(_context);

    public IRepository<Payment> Payments 
        => _paymentRepository ??= new Repository<Payment>(_context);

    public IRepository<Invoice> Invoices 
        => _invoiceRepository ??= new Repository<Invoice>(_context);

    public IRepository<OutboxEvent> OutboxEvents 
        => _outboxEventRepository ??= new Repository<OutboxEvent>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context?.Dispose();
}

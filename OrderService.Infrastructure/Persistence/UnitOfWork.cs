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

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IOrderRepository Orders
    {
        get
        {
            _orderRepository ??= new OrderRepository(_context);
            return _orderRepository;
        }
    }

    public IRepository<Payment> Payments
    {
        get
        {
            _paymentRepository ??= new Repository<Payment>(_context);
            return _paymentRepository;
        }
    }

    public IRepository<Invoice> Invoices
    {
        get
        {
            _invoiceRepository ??= new Repository<Invoice>(_context);
            return _invoiceRepository;
        }
    }

    public IRepository<OutboxEvent> OutboxEvents
    {
        get
        {
            _outboxEventRepository ??= new Repository<OutboxEvent>(_context);
            return _outboxEventRepository;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}

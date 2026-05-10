namespace OrderService.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<Order> GetByCustomerId(Guid customerId)
    {
        return _dbSet.AsNoTracking().Where(o => o.CustomerId == customerId);
    }

    public async Task<bool> TryTransitionToPaymentPendingAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var affectedRows = await _dbSet
            .Where(o => o.Id == orderId
                        && o.CustomerId == customerId
                        && (o.Status == OrderStatus.Draft || o.Status == OrderStatus.PaymentFailed))
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(o => o.Status, OrderStatus.PaymentPending),
                cancellationToken);

        return affectedRows == 1;
    }

    public async Task<Order?> GetByIdAndCustomerIdAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId, cancellationToken);
    }
}



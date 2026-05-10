namespace OrderService.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
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



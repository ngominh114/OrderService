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

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);

    public async Task<IEnumerable<Order>> SearchByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(o => o.CustomerId == customerId).ToListAsync(cancellationToken);
}

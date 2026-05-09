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

    public async Task<IEnumerable<Order>> SearchByCustomerIdAsync(
        Guid customerId,
        string? orderName = "",
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(o => o.CustomerId == customerId);

        if (!string.IsNullOrWhiteSpace(orderName))
            query = query.Where(o => o.DisplayName.Contains(orderName));

        return await query.ToListAsync(cancellationToken);
    }
}

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

    public async Task<IEnumerable<Order>> SearchAsync(
        OrderSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(o => o.CustomerId == criteria.CustomerId);

        if (!string.IsNullOrWhiteSpace(criteria.OrderName))
            query = query.Where(o => o.DisplayName.Contains(criteria.OrderName));

        if (criteria.Status.HasValue)
            query = query.Where(o => o.Status == criteria.Status);

        if (criteria.CreatedFrom.HasValue)
            query = query.Where(o => o.CreatedAt >= criteria.CreatedFrom);

        if (criteria.CreatedTo.HasValue)
            query = query.Where(o => o.CreatedAt <= criteria.CreatedTo);

        // Apply pagination
        query = query.Skip((criteria.Page - 1) * criteria.PageSize)
                     .Take(criteria.PageSize);

        return await query.ToListAsync(cancellationToken);
    }
}

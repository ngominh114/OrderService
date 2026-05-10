namespace OrderService.Domain.Interfaces;

using OrderService.Domain.Entities;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> SearchAsync(OrderSearchCriteria criteria, CancellationToken cancellationToken = default);
}

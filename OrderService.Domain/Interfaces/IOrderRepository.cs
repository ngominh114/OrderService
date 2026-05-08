namespace OrderService.Domain.Interfaces;

using OrderService.Domain.Entities;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> SearchByCustomerNameAsync(string customerName, CancellationToken cancellationToken = default);
}

namespace OrderService.Domain.Interfaces;

using OrderService.Domain.Entities;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> SearchByCustomerIdAsync(Guid customerId, string? orderName = "", CancellationToken cancellationToken = default);
}

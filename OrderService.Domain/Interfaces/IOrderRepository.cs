namespace OrderService.Domain.Interfaces;

using OrderService.Domain.Entities;

public interface IOrderRepository : IRepository<Order>
{
    IQueryable<Order> GetByCustomerId(Guid customerId);
}

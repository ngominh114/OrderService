namespace OrderService.Domain.Interfaces;

using OrderService.Domain.Entities;

public interface IOrderRepository : IRepository<Order>
{
    IQueryable<Order> GetByCustomerId(Guid customerId);

    Task<bool> TryTransitionToPaymentPendingAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAndCustomerIdAsync(Guid customerId, Guid orderId, CancellationToken cancellationToken = default);
}

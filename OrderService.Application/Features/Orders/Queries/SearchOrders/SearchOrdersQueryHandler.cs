namespace OrderService.Application.Features.Orders.Queries.SearchOrders;

using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Interfaces;

public class SearchOrdersQueryHandler : IRequestHandler<SearchOrdersQuery, List<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<OrderDto>> Handle(SearchOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Orders.SearchByCustomerIdAsync(request.CustomerId ?? Guid.Empty, cancellationToken);

        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            CustomerId = o.CustomerId,
            TotalAmount = o.TotalAmount,
            Status = o.Status.ToString(),
            CreatedAt = o.CreatedAt,
            CheckedOutAt = o.CheckedOutAt
        }).ToList();
    }
}

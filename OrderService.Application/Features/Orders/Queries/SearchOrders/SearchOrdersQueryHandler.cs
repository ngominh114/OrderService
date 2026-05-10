namespace OrderService.Application.Features.Orders.Queries.SearchOrders;

using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Mappings;
using OrderService.Domain.Entities;
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
        var criteria = new OrderSearchCriteria
        {
            CustomerId = request.CustomerId,
            OrderName = request.OrderName,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var orders = await _unitOfWork.Orders.SearchAsync(criteria, cancellationToken);

        return [.. orders.Select(o => o.ToDto())];
    }
}

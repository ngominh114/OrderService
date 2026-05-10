namespace OrderService.Application.Features.Orders.Queries.SearchOrders;

using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Mappings;
using OrderService.Domain.Interfaces;

public class SearchOrdersQueryHandler : IRequestHandler<SearchOrdersQuery, PaginatedResult<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<OrderDto>> Handle(SearchOrdersQuery request, CancellationToken cancellationToken)
    {
        // Get all orders for the customer with applied filters (still in-memory for now)
        var query = _unitOfWork.Orders.GetByCustomerId(request.CustomerId);

        // Apply optional filters in-memory (could be done at repository level if needed)
        var filteredOrders = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.OrderName))
            filteredOrders = filteredOrders.Where(o => o.DisplayName.Contains(request.OrderName));

        if (request.Status.HasValue)
            filteredOrders = filteredOrders.Where(o => o.Status == request.Status);

        if (request.CreatedFrom.HasValue)
            filteredOrders = filteredOrders.Where(o => o.CreatedAt >= request.CreatedFrom);

        if (request.CreatedTo.HasValue)
            filteredOrders = filteredOrders.Where(o => o.CreatedAt <= request.CreatedTo);

        // Get total count before pagination
        var totalCount = filteredOrders.Count();

        // Apply pagination and map to DTO
        var items = filteredOrders
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => o.ToDto())
            .ToList();

        return new PaginatedResult<OrderDto>(items, totalCount, request.Page, request.PageSize);
    }
}


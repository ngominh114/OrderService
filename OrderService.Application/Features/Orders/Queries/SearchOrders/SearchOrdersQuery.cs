namespace OrderService.Application.Features.Orders.Queries.SearchOrders;

using MediatR;
using OrderService.Application.Common;
using OrderService.Application.DTOs;

public class SearchOrdersQuery : IRequest<Result<List<OrderDto>>>
{
    public string? CustomerName { get; set; }
}

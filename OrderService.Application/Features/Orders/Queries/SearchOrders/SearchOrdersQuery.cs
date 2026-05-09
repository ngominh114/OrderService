namespace OrderService.Application.Features.Orders.Queries.SearchOrders;

using MediatR;
using OrderService.Application.DTOs;

public class SearchOrdersQuery : IRequest<List<OrderDto>>
{
    public Guid CustomerId { get; set; }
    public string? OrderName { get; set; }
}

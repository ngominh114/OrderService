namespace OrderService.Application.Features.Orders.Queries.SearchOrders;

using MediatR;
using OrderService.Application.DTOs;

public class SearchOrdersQuery : IRequest<List<OrderDto>>
{
    public Guid CustomerId { get; set; }
    public string? OrderName { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public void Normalize()
    {
        if (Page < 1)
            Page = 1;

        if (PageSize < 1)
            PageSize = 20;
        else if (PageSize > 100)
            PageSize = 100;

        if (!string.IsNullOrWhiteSpace(OrderName))
            OrderName = OrderName.Trim();
        else
            OrderName = null;
    }
}

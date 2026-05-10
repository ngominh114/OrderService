namespace OrderService.Application.Features.Orders.Queries.SearchOrders;

using System.ComponentModel.DataAnnotations;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;

public class SearchOrdersQuery : IRequest<PaginatedResult<OrderDto>>
{
    public Guid CustomerId { get; set; }

    [StringLength(500)]
    public string? OrderName { get; set; }

    public OrderStatus? Status { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

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

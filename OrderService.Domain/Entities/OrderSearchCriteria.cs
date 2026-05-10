namespace OrderService.Domain.Entities;

using OrderService.Domain.Enums;

public sealed class OrderSearchCriteria
{
    public Guid CustomerId { get; set; }
    public string? OrderName { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

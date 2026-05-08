namespace OrderService.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Orders.Commands.CheckoutOrder;
using OrderService.Application.Features.Orders.Queries.SearchOrders;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        var query = new SearchOrdersQuery { CustomerId = customerId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

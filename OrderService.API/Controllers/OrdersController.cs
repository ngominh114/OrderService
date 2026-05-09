namespace OrderService.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Orders.Commands.CheckoutOrder;
using OrderService.Application.Features.Orders.Queries.SearchOrders;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe([FromQuery] string? orderNumber, CancellationToken cancellationToken)
    {
        var customerId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(customerId))
            return Unauthorized("Customer ID not found in token");

        var query = new SearchOrdersQuery
        {
            CustomerId = Guid.Parse(customerId),
            OrderName = orderNumber
        };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("checkout")]
    [AllowAnonymous]
    public async Task<IActionResult> Checkout([FromBody] CheckoutOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

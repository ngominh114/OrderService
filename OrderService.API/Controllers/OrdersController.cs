namespace OrderService.API.Controllers;

using Asp.Versioning;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.Constants;
using OrderService.Application.Features.Orders.Commands.CheckoutOrder;
using OrderService.Application.Features.Orders.Queries.SearchOrders;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(ApiVersions.V1)]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe([FromQuery] string? orderName, CancellationToken cancellationToken)
    {
        var customerId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
        if (string.IsNullOrEmpty(customerId))
            return Unauthorized("Customer ID not found in token");

        var query = new SearchOrdersQuery
        {
            CustomerId = Guid.Parse(customerId),
            OrderName = orderName
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

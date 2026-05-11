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
[Authorize(Policy = AuthorizationPolicies.OrderRead)]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(
        [FromQuery] string? orderName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var customerId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
        if (string.IsNullOrEmpty(customerId))
            return Unauthorized("Customer ID not found in token");

        if (!Guid.TryParse(customerId, out var customerGuid))
            return Unauthorized("Invalid customer ID in token");

        var query = new SearchOrdersQuery
        {
            CustomerId = customerGuid,
            OrderName = orderName,
            Page = page,
            PageSize = pageSize
        };
        query.Normalize();

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("checkout")]
    [Authorize(Policy = AuthorizationPolicies.OrderCheckout)]
    public async Task<IActionResult> Checkout(
        [FromBody] CheckoutOrderCommand command,
        CancellationToken cancellationToken)
    {
        var customerId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
        if (string.IsNullOrEmpty(customerId))
            return Unauthorized("Customer ID not found in token");

        if (!Guid.TryParse(customerId, out var customerGuid))
            return Unauthorized("Invalid customer ID in token");

        command.CustomerId = customerGuid;

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}



namespace OrderService.API.Extensions;

using OrderService.API.Constants;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        return services.AddAuthorizationBuilder()
            .AddPolicy("OrderRead", policy =>
                policy.RequireClaim("scope", ApiScopes.OrderRead))
            .AddPolicy("OrderCheckout", policy =>
                policy.RequireClaim("scope", ApiScopes.OrderCheckout))
            .Services;
    }
}

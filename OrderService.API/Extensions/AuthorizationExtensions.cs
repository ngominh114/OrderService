namespace OrderService.API.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrderService.API.Authorization;
using OrderService.API.Constants;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddScoped<JwtBearerValidationEvents>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = JwtBearerValidationEvents.ValidIssuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
                options.EventsType = typeof(JwtBearerValidationEvents);
            });

        return services.AddAuthorizationBuilder()
            .AddPolicy("OrderRead", policy =>
                policy.RequireClaim("scope", ApiScopes.OrderRead))
            .AddPolicy("OrderCheckout", policy =>
                policy.RequireClaim("scope", ApiScopes.OrderCheckout))
            .Services;
    }
}

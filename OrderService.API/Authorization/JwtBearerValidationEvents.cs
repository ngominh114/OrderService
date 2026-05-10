namespace OrderService.API.Authorization;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class JwtBearerValidationEvents : JwtBearerEvents
{
    public const string ValidIssuer = "https://auth.orderservice.local";

    public override Task TokenValidated(TokenValidatedContext context)
    {
        var tokenIssuer = (context.SecurityToken as JwtSecurityToken)?.Issuer;

        if (!string.Equals(tokenIssuer, ValidIssuer, StringComparison.Ordinal))
        {
            context.Fail("Token issuer is invalid.");
        }

        return Task.CompletedTask;
    }
}

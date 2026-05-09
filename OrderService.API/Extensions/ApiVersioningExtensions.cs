namespace OrderService.API.Extensions;

using Asp.Versioning;
using OrderService.API.Constants;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(int.Parse(ApiVersions.V1));
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new QueryStringApiVersionReader("apiVersion");
        })
        .AddMvc();

        return services;
    }
}

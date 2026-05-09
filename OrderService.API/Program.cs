using OrderService.API.Extensions;
using OrderService.Application.Extensions;
using OrderService.Infrastructure.Extensions;
using OrderService.Infrastructure.Persistence;
using OrderService.API.Workers;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.AddLogging();

// Add API versioning
builder.Services.AddApiVersioningServices();

// Add services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();

// Add authorization policies for JWT scopes
builder.Services.AddAuthorizationPolicies();

// Single unified outbox worker
builder.Services.AddHostedService<OutboxWorker>();

var app = builder.Build();

// Initialize database - apply migrations and ensure database is created
await app.InitializeDatabaseAsync();

app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

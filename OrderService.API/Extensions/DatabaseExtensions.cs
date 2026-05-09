namespace OrderService.API.Extensions;

using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Persistence;
using Serilog;

public static class DatabaseExtensions
{
    /// <summary>
    /// Initializes the database by applying pending migrations.
    /// Should be called after building the application but before running it.
    /// </summary>
    /// <param name="app">The WebApplication instance</param>
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            Log.Information("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to apply database migrations");
            throw;
        }
    }
}

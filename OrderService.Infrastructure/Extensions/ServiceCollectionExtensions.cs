namespace OrderService.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Payments;
using OrderService.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMessageQueueService, MessageQueueService>();
        services.AddScoped<MockPaymentProcessor>();
        services.AddScoped<PaypalPaymentProcessor>();
        services.AddScoped<MomoPaymentProcessor>();
        services.AddScoped<IPaymentProcessorFactory, PaymentProcessorFactory>();

        return services;
    }
}

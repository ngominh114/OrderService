namespace OrderService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<OutboxEvent> OutboxEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Money as an owned type for Order.Cost
        modelBuilder.Entity<Order>()
            .OwnsOne(o => o.Cost, money =>
            {
                money.Property(m => m.Amount).HasColumnName("CostAmount");
                money.Property(m => m.Currency).HasColumnName("CostCurrency");
            });

        // Configure Money as an owned type for Payment.Amount
        modelBuilder.Entity<Payment>()
            .OwnsOne(p => p.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("PaymentAmount");
                money.Property(m => m.Currency).HasColumnName("PaymentCurrency");
            });

        // Configure Money as an owned type for Invoice.Amount
        modelBuilder.Entity<Invoice>()
            .OwnsOne(i => i.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("InvoiceAmount");
                money.Property(m => m.Currency).HasColumnName("InvoiceCurrency");
            });
    }
}

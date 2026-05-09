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

        // ===== CUSTOMER CONFIGURATION =====
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Customer_Email");

            entity.HasMany(e => e.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== ORDER CONFIGURATION =====
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .IsRequired();

            entity.Property(e => e.CustomerId)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.ImageIds)
                .IsRequired()
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(",", System.StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToArray());

            // Configure Money as an owned type for Order.Cost
            entity.OwnsOne(e => e.Cost, money =>
            {
                money.Property(m => m.Amount).HasColumnName("CostAmount").HasPrecision(18, 2).IsRequired();
                money.Property(m => m.Currency).HasColumnName("CostCurrency").HasMaxLength(3).IsRequired();
            });

            // Foreign key relationship with Customer
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-one relationship with Payment
            entity.HasOne(e => e.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-one relationship with Invoice
            entity.HasOne(e => e.Invoice)
                .WithOne(i => i.Order)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite index: (CustomerId, DisplayName) optimal for LIKE '%name%' queries
            entity.HasIndex(e => new { e.CustomerId, e.DisplayName })
                .IsUnique(false)
                .HasDatabaseName("IX_Order_CustomerId_DisplayName");
        });

        // ===== PAYMENT CONFIGURATION =====
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OrderId)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired();

            entity.Property(e => e.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.TransactionId)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.FailureReason)
                .HasMaxLength(500);

            // Configure Money as an owned type for Payment.Amount
            entity.OwnsOne(e => e.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("PaymentAmount").HasPrecision(18, 2).IsRequired();
                money.Property(m => m.Currency).HasColumnName("PaymentCurrency").HasMaxLength(3).IsRequired();
            });

            // Foreign key relationship with Order
            entity.HasOne(e => e.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index on OrderId for lookups
            entity.HasIndex(e => e.OrderId)
                .IsUnique()
                .HasDatabaseName("IX_Payment_OrderId");
        });

        // ===== INVOICE CONFIGURATION =====
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OrderId)
                .IsRequired();

            entity.Property(e => e.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.IssuedAt)
                .IsRequired();

            entity.Property(e => e.FilePath)
                .HasMaxLength(500);

            // Configure Money as an owned type for Invoice.Amount
            entity.OwnsOne(e => e.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("InvoiceAmount").HasPrecision(18, 2).IsRequired();
                money.Property(m => m.Currency).HasColumnName("InvoiceCurrency").HasMaxLength(3).IsRequired();
            });

            // Foreign key relationship with Order
            entity.HasOne(e => e.Order)
                .WithOne(o => o.Invoice)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index on OrderId for lookups
            entity.HasIndex(e => e.OrderId)
                .IsUnique()
                .HasDatabaseName("IX_Invoice_OrderId");

            // Unique index on InvoiceNumber
            entity.HasIndex(e => e.InvoiceNumber)
                .IsUnique()
                .HasDatabaseName("IX_Invoice_InvoiceNumber");
        });

        // ===== OUTBOX EVENT CONFIGURATION =====
        modelBuilder.Entity<OutboxEvent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AggregateId)
                .IsRequired();

            entity.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Payload)
                .IsRequired();

            entity.Property(e => e.IsProcessed)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.ProcessedAt);

            // Filtered index: (IsProcessed, CreatedAt) WHERE IsProcessed = 0
            // - Filtered index only contains ~1% of records (unprocessed events only)
            // - Seeks via IsProcessed = 0, then orders by CreatedAt for FIFO processing
            // - Superior to (IsProcessed, Id): CreatedAt enables time-based retention policies
            // - Query: SELECT * FROM OutboxEvents WHERE IsProcessed = 0 ORDER BY CreatedAt ASC
            // - Benefits: Smaller index size, faster inserts (don't index processed records), optimal for polling
            entity.HasIndex(e => new { e.IsProcessed, e.CreatedAt })
                .HasDatabaseName("IX_OutboxEvent_IsProcessed_CreatedAt")
                .HasFilter("[IsProcessed] = 0");

            // Single column index on AggregateId for event retrieval by aggregate root
            // - Enables efficient event sourcing: "Find all events for this Order aggregate"
            // - Query: SELECT * FROM OutboxEvents WHERE AggregateId = @orderId ORDER BY CreatedAt
            // - Use case: Replaying events, auditing, order history
            entity.HasIndex(e => e.AggregateId)
                .HasDatabaseName("IX_OutboxEvent_AggregateId");
        });
    }
}

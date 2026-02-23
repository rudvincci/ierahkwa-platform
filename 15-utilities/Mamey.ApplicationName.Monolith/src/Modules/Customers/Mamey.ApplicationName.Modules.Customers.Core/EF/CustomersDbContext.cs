using Mamey.ApplicationName.Modules.Customers.Core.Domain.Entities;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Bank.Modules.Customers.Infrastructure.EF;

internal sealed class CustomersDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customers");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}

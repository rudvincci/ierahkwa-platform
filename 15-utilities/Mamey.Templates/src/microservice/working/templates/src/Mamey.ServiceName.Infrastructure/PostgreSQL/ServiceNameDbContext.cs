using Mamey.MessageBrokers.Outbox.Messages;
using Mamey.ServiceName.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mamey.ServiceName.Infrastructure.PostgreSQL.Configuration;
using Mamey.MessageBrokers.Outbox.Messages;
using Mamey.Persistence.SQL;
using System.Text.Json;

namespace Mamey.ServiceName.Infrastructure.PostgreSQL;

internal class ServiceNameDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<EntityName> EntityNames { get; set; }

    public ServiceNameDbContext(DbContextOptions<ServiceNameDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.HasDefaultSchema("entityName");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        modelBuilder.Entity<InboxMessage>().ToTable("inbox");
        modelBuilder.Entity<OutboxMessage>().ToTable("outbox");

        modelBuilder.Entity<OutboxMessage>()
                .Property(b => b.Headers)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonExtensions.SerializerOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonExtensions.SerializerOptions)!
                )
                .Metadata.SetValueComparer(new DictionaryValueComparer());
            
        modelBuilder.Entity<OutboxMessage>()
                .Property(b => b.Message)
                .HasConversion<JsonValueConverter<object>>();
        modelBuilder.Entity<OutboxMessage>()
                .Property(b => b.MessageContext)
                .HasConversion<JsonValueConverter<object>>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceNameDbContext).Assembly);
        modelBuilder.UseSnakeCaseNamingConvention();
        modelBuilder.ApplyUtcDateTimeConverter();
        #if DEBUG
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                Console.WriteLine($"Entity: {entity.Name}, Table: {entity.GetTableName()}, Schema: {entity.GetSchema()}");
            }
        #endif
    }
}

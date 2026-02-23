using Mamey.MessageBrokers.Outbox.Messages;
using Pupitre.GLEs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mamey.Persistence.SQL;
using JsonExtensions = Mamey.JsonExtensions;
using System.Text.Json;

namespace Pupitre.GLEs.Infrastructure.PostgreSQL;

internal class GLEsDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<GLE> GLEs { get; set; }

    public GLEsDbContext(DbContextOptions<GLEsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.HasDefaultSchema("entityName");

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
        modelBuilder.UseSnakeCaseNamingConvention();
        modelBuilder.ApplyUtcDateTimeConverter();
    }
}

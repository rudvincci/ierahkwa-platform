using Mamey.MessageBrokers.Outbox.Messages;
using Pupitre.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mamey.Persistence.SQL;
using System.Text.Json;
using JsonExtensions = Mamey.JsonExtensions;

namespace Pupitre.Users.Infrastructure.PostgreSQL;

internal class UsersDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<User> Users { get; set; }

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.HasDefaultSchema("user");
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        modelBuilder.UseSnakeCaseNamingConvention();
        modelBuilder.ApplyUtcDateTimeConverter();
    }
}

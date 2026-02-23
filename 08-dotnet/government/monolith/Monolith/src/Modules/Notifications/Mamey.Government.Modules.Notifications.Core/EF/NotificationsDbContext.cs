using System.Text.Json;
using Mamey;
using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.Persistence.SQL;
using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Modules.Notifications.Core.EF
{
    internal sealed class NotificationsDbContext : DbContext
    {
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<User> Users { get; set; }

        public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("notifications");
            
            // Apply entity configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<InboxMessage>().ToTable("inbox");
            modelBuilder.Entity<OutboxMessage>().ToTable("outbox");

            modelBuilder.Entity<OutboxMessage>()
                .Property(b => b.Headers)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonExtensions.SerializerOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonExtensions.SerializerOptions)!
                )
                .Metadata.SetValueComparer(new DictionaryValueComparer());
            
            modelBuilder.UseSnakeCaseNamingConvention();
            modelBuilder.ApplyUtcDateTimeConverter();
        }
    }
}
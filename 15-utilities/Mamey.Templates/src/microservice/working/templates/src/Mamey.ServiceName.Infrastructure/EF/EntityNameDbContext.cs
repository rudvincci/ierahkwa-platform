using System.Text.Json;
using Mamey.MessageBrokers.Outbox.Messages;
using Mamey.Persistence.SQL;
using Mamey.ServiceName.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.ServiceName.Infrastructure.EF;

internal class EntityNameDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<EntityName> EntityNames { get; set; }

    public EntityNameDbContext(DbContextOptions<EntityNameDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<OutboxMessage>()
            .Property(b => b.Message)
            .HasConversion<JsonValueConverter<object>>();
        modelBuilder.Entity<OutboxMessage>()
            .Property(b => b.MessageContext)
            .HasConversion<JsonValueConverter<object>>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EntityNameDbContext).Assembly);

        // if (_tenant.TryGetTenantId(out var tenantId) && tenantId.Value != Guid.Empty)
        // {
        //     foreach (var entityType in mb.Model.GetEntityTypes()
        //         .Where(t => typeof(ITenantScoped).IsAssignableFrom(t.ClrType)))
        //     {
        //         var param = Expression.Parameter(entityType.ClrType, "e");
        //         var body = Expression.Equal(
        //             Expression.Property(
        //                 Expression.Property(
        //                     Expression.Convert(param, typeof(ITenantScoped)),
        //                     nameof(ITenantScoped.TenantId)),
        //                 nameof(TenantId.Value)),
        //             Expression.Constant(tenantId.Value));
        //
        //         entityType.SetQueryFilter(Expression.Lambda(body, param));
        //     }
        // }
        modelBuilder.UseSnakeCaseNamingConvention();
        modelBuilder.ApplyUtcDateTimeConverter();

#if DEBUG
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            Console.WriteLine($"Entity: {entity.Name}, Table: {entity.GetTableName()}, Schema: {entity.GetSchema()}");
        }
#endif
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
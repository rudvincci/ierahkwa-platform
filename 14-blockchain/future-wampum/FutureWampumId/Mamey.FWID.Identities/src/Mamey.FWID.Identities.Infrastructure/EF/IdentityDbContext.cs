using System.Text.Json;
using Mamey.MessageBrokers.Outbox.Messages;
using Mamey.Persistence.SQL;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF;

internal class IdentityDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    public DbSet<Identity> Identitys { get; set; }
    public DbSet<PermissionMapping> PermissionMappings { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<MfaConfiguration> MfaConfigurations { get; set; }
    public DbSet<IdentityRole> IdentityRoles { get; set; }
    public DbSet<IdentityPermission> IdentityPermissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<EmailConfirmation> EmailConfirmations { get; set; }
    public DbSet<SmsConfirmation> SmsConfirmations { get; set; }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // Suppress pending model changes warning
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
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
        
        // Explicitly ignore value object types that EF might discover as entities
        // These are configured via HasConversion in their respective entity configurations
        modelBuilder.Ignore<Mamey.FWID.Identities.Domain.Entities.IdentityId>();
        modelBuilder.Ignore<Mamey.FWID.Identities.Domain.Entities.EmailConfirmationId>();
        modelBuilder.Ignore<Mamey.FWID.Identities.Domain.Entities.SmsConfirmationId>();
        modelBuilder.Ignore<Mamey.FWID.Identities.Domain.Entities.SessionId>();
        modelBuilder.Ignore<Mamey.FWID.Identities.Domain.Entities.MfaConfigurationId>();
        modelBuilder.Ignore<Mamey.Types.RoleId>();
        modelBuilder.Ignore<Mamey.FWID.Identities.Domain.Entities.PermissionId>();
        
        // Configure strong ID types using HasConversion extension
        modelBuilder.SetStrongIdsGuidClass(
            typeof(Mamey.FWID.Identities.Domain.Entities.IdentityId),
            typeof(Mamey.FWID.Identities.Domain.Entities.EmailConfirmationId),
            typeof(Mamey.FWID.Identities.Domain.Entities.SmsConfirmationId),
            typeof(Mamey.FWID.Identities.Domain.Entities.SessionId),
            typeof(Mamey.FWID.Identities.Domain.Entities.MfaConfigurationId),
            typeof(Mamey.Types.RoleId),
            typeof(Mamey.FWID.Identities.Domain.Entities.PermissionId)
        );
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

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
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
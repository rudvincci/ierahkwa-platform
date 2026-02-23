using System.Text.Json;
using Mamey;
using Mamey.Government.Modules.Tenant.Core.EF.Configuration;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.Persistence.SQL;
using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Modules.Tenant.Core.EF;

internal class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
    {
    }

    public DbSet<TenantRow> Tenants { get; set; } = null!;
    public DbSet<TenantSettingsRow> TenantSettings { get; set; } = null!;
    public DbSet<TenantUserMappingRow> TenantUserMappings { get; set; } = null!;
    public DbSet<TenantInviteRow> TenantInvites { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tenant");
        
        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
        
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

internal class TenantRow
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}

internal class TenantSettingsRow
{
    public Guid TenantId { get; set; }
    public string BrandingJson { get; set; } = "{}";
    public string? ActivationJson { get; set; }
    public string? DocumentNamingConfig { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

internal class TenantUserMappingRow
{
    public string Issuer { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

internal class TenantInviteRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
}

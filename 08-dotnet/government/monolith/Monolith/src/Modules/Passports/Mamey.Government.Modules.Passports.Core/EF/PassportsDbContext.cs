using System.Text.Json;
using Mamey;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.EF.Configuration;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.Persistence.SQL;
using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Modules.Passports.Core.EF;

internal class PassportsDbContext : DbContext
{
    public PassportsDbContext(DbContextOptions<PassportsDbContext> options) : base(options)
    {
    }

    public DbSet<PassportRow> Passports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("passports");
        
        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PassportsDbContext).Assembly);
        
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

internal class PassportRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CitizenId { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? Mrz { get; set; }
    public string? DocumentPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}

using System.Text.Json;
using Mamey;
using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.EF.Configuration;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.Persistence.SQL;
using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Modules.Certificates.Core.EF;

internal class CertificatesDbContext : DbContext
{
    public CertificatesDbContext(DbContextOptions<CertificatesDbContext> options) : base(options)
    {
    }

    public DbSet<CertificateRow> Certificates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("certificates");
        
        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CertificatesDbContext).Assembly);
        
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

internal class CertificateRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? CitizenId { get; set; }
    public CertificateType CertificateType { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public string? DocumentPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}

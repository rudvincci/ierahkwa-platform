using System.Text.Json;
using Mamey;
using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Documents.Core.EF.Configuration;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.Persistence.SQL;
using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Modules.Documents.Core.EF;

internal class DocumentsDbContext : DbContext
{
    public DocumentsDbContext(DbContextOptions<DocumentsDbContext> options) : base(options)
    {
    }

    public DbSet<DocumentRow> Documents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("documents");
        
        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentsDbContext).Assembly);
        
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

internal class DocumentRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StorageBucket { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? MetadataJson { get; set; }
    public bool IsActive { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}

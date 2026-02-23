using System.Text.Json;
using Mamey;
using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.EF.Configuration;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.Persistence.SQL;
using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Modules.Citizens.Core.EF;

internal class CitizensDbContext : DbContext
{
    public CitizensDbContext(DbContextOptions<CitizensDbContext> options) : base(options)
    {
    }

    public DbSet<CitizenRow> Citizens { get; set; } = null!;
    public DbSet<CitizenshipStatusHistoryRow> StatusHistory { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("citizens");
        
        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CitizensDbContext).Assembly);
        
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

internal class CitizenRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AddressJson { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public CitizenshipStatus Status { get; set; }
    public string? PhotoPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
    
    // Navigation property for status history
    public List<CitizenshipStatusHistoryRow> StatusHistory { get; set; } = new();
}

internal class CitizenshipStatusHistoryRow
{
    public Guid CitizenId { get; set; }
    public CitizenshipStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Reason { get; set; }
}

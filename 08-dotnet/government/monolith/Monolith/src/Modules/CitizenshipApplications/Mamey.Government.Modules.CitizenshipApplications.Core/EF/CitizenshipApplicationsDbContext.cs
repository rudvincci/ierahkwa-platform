using System.Text.Json;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Core.EF.Configuration;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.Persistence.SQL;
using Mamey.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF;

internal class CitizenshipApplicationsDbContext : DbContext
{
    public CitizenshipApplicationsDbContext(DbContextOptions<CitizenshipApplicationsDbContext> options) : base(options)
    {
    }

    public DbSet<CitizenshipApplication> Applications { get; set; } = null!;
    public DbSet<UploadedDocument> UploadedDocuments { get; set; } = null!;
    public DbSet<ApplicationToken> ApplicationTokens { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("citizenship_applications");
        
        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CitizenshipApplicationsDbContext).Assembly);
        
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


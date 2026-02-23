using Microsoft.EntityFrameworkCore;
using Mamey.Auth.Decentralized.Persistence.Write.Entities;

namespace Mamey.Auth.Decentralized.Persistence.Write;

/// <summary>
/// DbContext for DID-related entities in the write database
/// </summary>
public class DidDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the DidDbContext class
    /// </summary>
    /// <param name="options">The DbContext options</param>
    public DidDbContext(DbContextOptions<DidDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DID Documents
    /// </summary>
    public DbSet<DidDocumentEntity> DidDocuments { get; set; } = null!;

    /// <summary>
    /// Verification Methods
    /// </summary>
    public DbSet<VerificationMethodEntity> VerificationMethods { get; set; } = null!;

    /// <summary>
    /// Service Endpoints
    /// </summary>
    public DbSet<ServiceEndpointEntity> ServiceEndpoints { get; set; } = null!;

    /// <summary>
    /// Configures the model
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DidDocumentEntity
        modelBuilder.Entity<DidDocumentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Did).IsUnique();
            entity.HasIndex(e => new { e.Did, e.Method });
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.UpdatedAt);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Did)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Method)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Document)
                .IsRequired();

            entity.Property(e => e.DocumentHash)
                .HasMaxLength(64);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });

        // Configure VerificationMethodEntity
        modelBuilder.Entity<VerificationMethodEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DidDocumentId);
            entity.HasIndex(e => e.VerificationMethodId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.VerificationMethodId)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Controller)
                .HasMaxLength(500);

            entity.Property(e => e.PublicKeyMaterial)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Configure relationship
            entity.HasOne(e => e.DidDocument)
                .WithMany()
                .HasForeignKey(e => e.DidDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ServiceEndpointEntity
        modelBuilder.Entity<ServiceEndpointEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DidDocumentId);
            entity.HasIndex(e => e.ServiceEndpointId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.ServiceEndpointId)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ServiceEndpoint)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Configure relationship
            entity.HasOne(e => e.DidDocument)
                .WithMany()
                .HasForeignKey(e => e.DidDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Saves changes and updates timestamps
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of affected records</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Saves changes and updates timestamps
    /// </summary>
    /// <returns>The number of affected records</returns>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is DidDocumentEntity didDocument)
            {
                if (entry.State == EntityState.Added)
                {
                    didDocument.CreatedAt = DateTime.UtcNow;
                }
                didDocument.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is VerificationMethodEntity verificationMethod)
            {
                if (entry.State == EntityState.Added)
                {
                    verificationMethod.CreatedAt = DateTime.UtcNow;
                }
                verificationMethod.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is ServiceEndpointEntity serviceEndpoint)
            {
                if (entry.State == EntityState.Added)
                {
                    serviceEndpoint.CreatedAt = DateTime.UtcNow;
                }
                serviceEndpoint.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

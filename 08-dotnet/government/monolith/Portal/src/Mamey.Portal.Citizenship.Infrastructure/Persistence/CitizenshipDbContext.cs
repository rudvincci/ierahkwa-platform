using Microsoft.EntityFrameworkCore;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence;

public sealed class CitizenshipDbContext : DbContext
{
    public CitizenshipDbContext(DbContextOptions<CitizenshipDbContext> options) : base(options)
    {
    }

    public DbSet<CitizenshipApplicationRow> Applications => Set<CitizenshipApplicationRow>();
    public DbSet<CitizenshipUploadRow> Uploads => Set<CitizenshipUploadRow>();
    public DbSet<CitizenshipIssuedDocumentRow> IssuedDocuments => Set<CitizenshipIssuedDocumentRow>();
    public DbSet<CitizenshipStatusRow> CitizenshipStatuses => Set<CitizenshipStatusRow>();
    public DbSet<StatusProgressionApplicationRow> StatusProgressionApplications => Set<StatusProgressionApplicationRow>();
    public DbSet<IntakeReviewRow> IntakeReviews => Set<IntakeReviewRow>();
    public DbSet<PaymentPlanRow> PaymentPlans => Set<PaymentPlanRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CitizenshipApplicationRow>(b =>
        {
            b.ToTable("citizenship_applications");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.TenantId, x.ApplicationNumber }).IsUnique();

            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.ApplicationNumber).HasMaxLength(64).IsRequired();
            b.Property(x => x.Status).HasMaxLength(64).IsRequired();

            b.Property(x => x.FirstName).HasMaxLength(128).IsRequired();
            b.Property(x => x.LastName).HasMaxLength(128).IsRequired();
            b.Property(x => x.Email).HasMaxLength(256);

            // AAMVA-compliant fields
            b.Property(x => x.MiddleName).HasMaxLength(128);
            b.Property(x => x.Height).HasMaxLength(32);
            b.Property(x => x.EyeColor).HasMaxLength(32);
            b.Property(x => x.HairColor).HasMaxLength(32);
            b.Property(x => x.Sex).HasMaxLength(16);

            // Additional fields
            b.Property(x => x.PhoneNumber).HasMaxLength(64);
            b.Property(x => x.PlaceOfBirth).HasMaxLength(256);
            b.Property(x => x.CountryOfOrigin).HasMaxLength(128);
            b.Property(x => x.MaritalStatus).HasMaxLength(64);
            b.Property(x => x.PreviousNames).HasMaxLength(512);

            // Address
            b.Property(x => x.AddressLine1).HasMaxLength(256);
            b.Property(x => x.City).HasMaxLength(128);
            b.Property(x => x.Region).HasMaxLength(128);
            b.Property(x => x.PostalCode).HasMaxLength(64);

            b.HasMany(x => x.Uploads)
                .WithOne()
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.IssuedDocuments)
                .WithOne()
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CitizenshipUploadRow>(b =>
        {
            b.ToTable("citizenship_uploads");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.ApplicationId, x.Kind });

            b.Property(x => x.Kind).HasMaxLength(64).IsRequired();
            b.Property(x => x.FileName).HasMaxLength(256).IsRequired();
            b.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
            b.Property(x => x.StorageBucket).HasMaxLength(128).IsRequired();
            b.Property(x => x.StorageKey).HasMaxLength(512).IsRequired();
        });

        modelBuilder.Entity<CitizenshipIssuedDocumentRow>(b =>
        {
            b.ToTable("citizenship_issued_documents");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.ApplicationId, x.Kind });
            b.HasIndex(x => x.DocumentNumber);

            b.Property(x => x.Kind).HasMaxLength(64).IsRequired();
            b.Property(x => x.DocumentNumber).HasMaxLength(64);
            b.Property(x => x.FileName).HasMaxLength(256).IsRequired();
            b.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
            b.Property(x => x.StorageBucket).HasMaxLength(128).IsRequired();
            b.Property(x => x.StorageKey).HasMaxLength(512).IsRequired();
        });

        modelBuilder.Entity<CitizenshipStatusRow>(b =>
        {
            b.ToTable("citizenship_statuses");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
            b.HasIndex(x => x.ApplicationId);

            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.Email).HasMaxLength(256).IsRequired();
            b.Property(x => x.Status).HasMaxLength(64).IsRequired();

            b.HasMany(x => x.ProgressionApplications)
                .WithOne()
                .HasForeignKey(x => x.CitizenshipStatusId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StatusProgressionApplicationRow>(b =>
        {
            b.ToTable("status_progression_applications");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.TenantId, x.ApplicationNumber }).IsUnique();
            b.HasIndex(x => x.CitizenshipStatusId);

            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.ApplicationNumber).HasMaxLength(64).IsRequired();
            b.Property(x => x.TargetStatus).HasMaxLength(64).IsRequired();
            b.Property(x => x.Status).HasMaxLength(64).IsRequired();
        });

        modelBuilder.Entity<IntakeReviewRow>(b =>
        {
            b.ToTable("intake_reviews");
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.ApplicationId);
            b.HasIndex(x => new { x.TenantId, x.ApplicationId });

            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.ReviewerName).HasMaxLength(256).IsRequired();
            b.Property(x => x.Recommendation).HasMaxLength(64).IsRequired();
            b.Property(x => x.RecommendationReason).HasMaxLength(2000);
            b.Property(x => x.CompletenessNotes).HasMaxLength(2000);
            b.Property(x => x.DocumentNotes).HasMaxLength(2000);
            b.Property(x => x.AdditionalNotes).HasMaxLength(2000);

            b.HasOne(x => x.Application)
                .WithMany()
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PaymentPlanRow>(b =>
        {
            b.ToTable("payment_plans");
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.ApplicationId);
            b.HasIndex(x => new { x.TenantId, x.ApplicationId }).IsUnique();

            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.ApplicationNumber).HasMaxLength(64).IsRequired();
            b.Property(x => x.Currency).HasMaxLength(8).IsRequired();
            b.Property(x => x.Status).HasMaxLength(64).IsRequired();
            b.Property(x => x.PaymentReference).HasMaxLength(256);
            b.Property(x => x.PaymentMethod).HasMaxLength(64);
            b.Property(x => x.PaymentGateway).HasMaxLength(64);

            b.HasOne(x => x.Application)
                .WithMany()
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}



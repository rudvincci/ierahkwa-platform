using Common.Application.Interfaces;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Receptionist.Domain.Entities;

namespace Receptionist.Persistence;

public class ReceptionistDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly ITenantService? _tenantService;

    public ReceptionistDbContext(DbContextOptions<ReceptionistDbContext> options) : base(options)
    {
    }

    public ReceptionistDbContext(
        DbContextOptions<ReceptionistDbContext> options,
        ICurrentUserService currentUserService,
        ITenantService tenantService) : base(options)
    {
        _currentUserService = currentUserService;
        _tenantService = tenantService;
    }

    public DbSet<AdmissionEnquiry> AdmissionEnquiries => Set<AdmissionEnquiry>();
    public DbSet<VisitorBook> VisitorBooks => Set<VisitorBook>();
    public DbSet<PhoneLog> PhoneLogs => Set<PhoneLog>();
    public DbSet<PostalDispatch> PostalDispatches => Set<PostalDispatch>();
    public DbSet<PostalReceive> PostalReceives => Set<PostalReceive>();
    public DbSet<Complain> Complains => Set<Complain>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdmissionEnquiry>(entity =>
        {
            entity.ToTable("AdmissionEnquiries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StudentName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ParentName).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Source).HasMaxLength(100);
        });

        modelBuilder.Entity<VisitorBook>(entity =>
        {
            entity.ToTable("VisitorBooks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VisitorName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.IdNumber).HasMaxLength(50);
            entity.Property(e => e.Purpose).HasMaxLength(500);
            entity.Property(e => e.PersonToMeet).HasMaxLength(200);
            entity.Property(e => e.Badge).HasMaxLength(50);
        });

        modelBuilder.Entity<PhoneLog>(entity =>
        {
            entity.ToTable("PhoneLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CallerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Purpose).HasMaxLength(500);
        });

        modelBuilder.Entity<PostalDispatch>(entity =>
        {
            entity.ToTable("PostalDispatches");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferenceNo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ToTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FromTitle).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.HasIndex(e => new { e.TenantId, e.ReferenceNo }).IsUnique();
        });

        modelBuilder.Entity<PostalReceive>(entity =>
        {
            entity.ToTable("PostalReceives");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferenceNo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FromTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ToTitle).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.HasIndex(e => new { e.TenantId, e.ReferenceNo }).IsUnique();
        });

        modelBuilder.Entity<Complain>(entity =>
        {
            entity.ToTable("Complains");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ComplainNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ComplainerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).IsRequired();
            entity.HasIndex(e => new { e.TenantId, e.ComplainNumber }).IsUnique();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantService?.GetCurrentTenantId();
        
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService?.UserName;
                    if (entry.Entity is TenantEntity tenantEntity && tenantId.HasValue)
                        tenantEntity.TenantId = tenantId.Value;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUserService?.UserName;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

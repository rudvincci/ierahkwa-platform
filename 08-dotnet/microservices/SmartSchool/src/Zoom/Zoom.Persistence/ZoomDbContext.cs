using Common.Application.Interfaces;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Zoom.Domain.Entities;

namespace Zoom.Persistence;

public class ZoomDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly ITenantService? _tenantService;

    public ZoomDbContext(DbContextOptions<ZoomDbContext> options) : base(options)
    {
    }

    public ZoomDbContext(
        DbContextOptions<ZoomDbContext> options,
        ICurrentUserService currentUserService,
        ITenantService tenantService) : base(options)
    {
        _currentUserService = currentUserService;
        _tenantService = tenantService;
    }

    public DbSet<LiveClass> LiveClasses => Set<LiveClass>();
    public DbSet<LiveClassAttendance> LiveClassAttendances => Set<LiveClassAttendance>();
    public DbSet<ZoomSettings> ZoomSettings => Set<ZoomSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LiveClass>(entity =>
        {
            entity.ToTable("LiveClasses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ZoomMeetingId).HasMaxLength(100);
            entity.Property(e => e.ZoomPassword).HasMaxLength(50);
        });

        modelBuilder.Entity<LiveClassAttendance>(entity =>
        {
            entity.ToTable("LiveClassAttendances");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.LiveClass)
                .WithMany(l => l.Attendances)
                .HasForeignKey(e => e.LiveClassId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.LiveClassId, e.StudentId }).IsUnique();
        });

        modelBuilder.Entity<ZoomSettings>(entity =>
        {
            entity.ToTable("ZoomSettings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ApiKey).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ApiSecret).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AccountId).HasMaxLength(200);
            entity.Property(e => e.ClientId).HasMaxLength(200);
            entity.Property(e => e.ClientSecret).HasMaxLength(200);
            entity.Property(e => e.WebhookSecret).HasMaxLength(200);
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

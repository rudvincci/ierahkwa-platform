using Microsoft.EntityFrameworkCore;

namespace Mamey.Portal.Cms.Infrastructure.Persistence;

public sealed class CmsDbContext : DbContext
{
    public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options)
    {
    }

    public DbSet<CmsNewsItemRow> News => Set<CmsNewsItemRow>();
    public DbSet<CmsPageRow> Pages => Set<CmsPageRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CmsNewsItemRow>(b =>
        {
            b.ToTable("cms_news");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.TenantId, x.CreatedAt });

            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.Title).HasMaxLength(256).IsRequired();
            b.Property(x => x.Summary).HasMaxLength(2048).IsRequired();
            b.Property(x => x.BodyHtml).HasMaxLength(65535).IsRequired();
            b.Property(x => x.Status).HasMaxLength(32).IsRequired();

            b.Property(x => x.CreatedBy).HasMaxLength(256);
            b.Property(x => x.UpdatedBy).HasMaxLength(256);
            b.Property(x => x.RejectionReason).HasMaxLength(2048);
        });

        modelBuilder.Entity<CmsPageRow>(b =>
        {
            b.ToTable("cms_pages");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique();

            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.Slug).HasMaxLength(128).IsRequired();
            b.Property(x => x.Title).HasMaxLength(256).IsRequired();
            b.Property(x => x.BodyHtml).HasMaxLength(65535).IsRequired();
            b.Property(x => x.Status).HasMaxLength(32).IsRequired();

            b.Property(x => x.CreatedBy).HasMaxLength(256);
            b.Property(x => x.UpdatedBy).HasMaxLength(256);
            b.Property(x => x.RejectionReason).HasMaxLength(2048);
        });
    }
}



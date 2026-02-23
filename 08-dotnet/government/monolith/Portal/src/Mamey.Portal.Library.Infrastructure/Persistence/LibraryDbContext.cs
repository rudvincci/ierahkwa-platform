using Microsoft.EntityFrameworkCore;

namespace Mamey.Portal.Library.Infrastructure.Persistence;

public sealed class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<LibraryItemRow> Items => Set<LibraryItemRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryItemRow>(b =>
        {
            b.ToTable("library_items");
            b.HasKey(x => x.Id);
            b.Property(x => x.TenantId).HasMaxLength(128).IsRequired();
            b.Property(x => x.Category).HasMaxLength(128).IsRequired();
            b.Property(x => x.Title).HasMaxLength(256).IsRequired();
            b.Property(x => x.Summary).HasMaxLength(2048);
            b.Property(x => x.Visibility).HasConversion<string>().HasMaxLength(32).IsRequired();
            b.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            b.Property(x => x.FileName).HasMaxLength(256).IsRequired();
            b.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
            b.Property(x => x.StorageBucket).HasMaxLength(128).IsRequired();
            b.Property(x => x.StorageKey).HasMaxLength(512).IsRequired();
            b.Property(x => x.CreatedBy).HasMaxLength(256);
            b.Property(x => x.UpdatedBy).HasMaxLength(256);

            b.HasIndex(x => new { x.TenantId, x.Status, x.Visibility, x.CreatedAt });
        });
    }
}





using CultureArchiveService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CultureArchiveService.Infrastructure;

public class CultureArchiveServiceDbContext : DbContext
{
    public CultureArchiveServiceDbContext(DbContextOptions<CultureArchiveServiceDbContext> options) : base(options) { }

    public DbSet<Artifact> Artifacts => Set<Artifact>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<CulturalSite> CulturalSites => Set<CulturalSite>();
    public DbSet<OralHistory> OralHistories => Set<OralHistory>();
    public DbSet<LanguageRecord> LanguageRecords => Set<LanguageRecord>();
    public DbSet<DigitalArchive> DigitalArchives => Set<DigitalArchive>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Artifact>().HasKey(e => e.Id);
        modelBuilder.Entity<Book>().HasKey(e => e.Id);
        modelBuilder.Entity<CulturalSite>().HasKey(e => e.Id);
        modelBuilder.Entity<OralHistory>().HasKey(e => e.Id);
        modelBuilder.Entity<LanguageRecord>().HasKey(e => e.Id);
        modelBuilder.Entity<DigitalArchive>().HasKey(e => e.Id);
    }
}

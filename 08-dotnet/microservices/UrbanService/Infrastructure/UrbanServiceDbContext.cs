using UrbanService.Domain;
using Microsoft.EntityFrameworkCore;

namespace UrbanService.Infrastructure;

public class UrbanServiceDbContext : DbContext
{
    public UrbanServiceDbContext(DbContextOptions<UrbanServiceDbContext> options) : base(options) { }

    public DbSet<LandParcel> LandParcels => Set<LandParcel>();
    public DbSet<UrbanPlan> UrbanPlans => Set<UrbanPlan>();
    public DbSet<HousingUnit> HousingUnits => Set<HousingUnit>();
    public DbSet<ZoningRegulation> ZoningRegulations => Set<ZoningRegulation>();
    public DbSet<ConstructionPermit> ConstructionPermits => Set<ConstructionPermit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<LandParcel>().HasKey(e => e.Id);
        modelBuilder.Entity<UrbanPlan>().HasKey(e => e.Id);
        modelBuilder.Entity<HousingUnit>().HasKey(e => e.Id);
        modelBuilder.Entity<ZoningRegulation>().HasKey(e => e.Id);
        modelBuilder.Entity<ConstructionPermit>().HasKey(e => e.Id);
    }
}

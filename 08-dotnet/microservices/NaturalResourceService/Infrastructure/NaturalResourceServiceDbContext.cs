using NaturalResourceService.Domain;
using Microsoft.EntityFrameworkCore;

namespace NaturalResourceService.Infrastructure;

public class NaturalResourceServiceDbContext : DbContext
{
    public NaturalResourceServiceDbContext(DbContextOptions<NaturalResourceServiceDbContext> options) : base(options) { }

    public DbSet<WaterSource> WaterSources => Set<WaterSource>();
    public DbSet<EnergyPlant> EnergyPlants => Set<EnergyPlant>();
    public DbSet<NuclearFacility> NuclearFacilities => Set<NuclearFacility>();
    public DbSet<MiningOperation> MiningOperations => Set<MiningOperation>();
    public DbSet<ResourceReserve> ResourceReserves => Set<ResourceReserve>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<WaterSource>().HasKey(e => e.Id);
        modelBuilder.Entity<EnergyPlant>().HasKey(e => e.Id);
        modelBuilder.Entity<NuclearFacility>().HasKey(e => e.Id);
        modelBuilder.Entity<MiningOperation>().HasKey(e => e.Id);
        modelBuilder.Entity<ResourceReserve>().HasKey(e => e.Id);
    }
}

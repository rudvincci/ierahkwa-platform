using AgricultureService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AgricultureService.Infrastructure;

public class AgricultureServiceDbContext : DbContext
{
    public AgricultureServiceDbContext(DbContextOptions<AgricultureServiceDbContext> options) : base(options) { }

    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Harvest> Harvests => Set<Harvest>();
    public DbSet<LivestockRecord> LivestockRecords => Set<LivestockRecord>();
    public DbSet<AquaculturePool> AquaculturePools => Set<AquaculturePool>();
    public DbSet<IrrigationZone> IrrigationZones => Set<IrrigationZone>();
    public DbSet<FoodSupplyChain> FoodSupplyChains => Set<FoodSupplyChain>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Farm>().HasKey(e => e.Id);
        modelBuilder.Entity<Harvest>().HasKey(e => e.Id);
        modelBuilder.Entity<LivestockRecord>().HasKey(e => e.Id);
        modelBuilder.Entity<AquaculturePool>().HasKey(e => e.Id);
        modelBuilder.Entity<IrrigationZone>().HasKey(e => e.Id);
        modelBuilder.Entity<FoodSupplyChain>().HasKey(e => e.Id);
    }
}

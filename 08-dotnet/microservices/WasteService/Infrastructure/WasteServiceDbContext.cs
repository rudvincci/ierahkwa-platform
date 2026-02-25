using WasteService.Domain;
using Microsoft.EntityFrameworkCore;

namespace WasteService.Infrastructure;

public class WasteServiceDbContext : DbContext
{
    public WasteServiceDbContext(DbContextOptions<WasteServiceDbContext> options) : base(options) { }

    public DbSet<WasteCollection> WasteCollections => Set<WasteCollection>();
    public DbSet<RecyclingCenter> RecyclingCenters => Set<RecyclingCenter>();
    public DbSet<WasteCategory> WasteCategories => Set<WasteCategory>();
    public DbSet<CollectionRoute> CollectionRoutes => Set<CollectionRoute>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<WasteCollection>().HasKey(e => e.Id);
        modelBuilder.Entity<RecyclingCenter>().HasKey(e => e.Id);
        modelBuilder.Entity<WasteCategory>().HasKey(e => e.Id);
        modelBuilder.Entity<CollectionRoute>().HasKey(e => e.Id);
    }
}

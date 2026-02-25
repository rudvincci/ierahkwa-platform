using TourismService.Domain;
using Microsoft.EntityFrameworkCore;

namespace TourismService.Infrastructure;

public class TourismServiceDbContext : DbContext
{
    public TourismServiceDbContext(DbContextOptions<TourismServiceDbContext> options) : base(options) { }

    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<TourPackage> TourPackages => Set<TourPackage>();
    public DbSet<NationalPark> NationalParks => Set<NationalPark>();
    public DbSet<Guide> Guides => Set<Guide>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Destination>().HasKey(e => e.Id);
        modelBuilder.Entity<Booking>().HasKey(e => e.Id);
        modelBuilder.Entity<TourPackage>().HasKey(e => e.Id);
        modelBuilder.Entity<NationalPark>().HasKey(e => e.Id);
        modelBuilder.Entity<Guide>().HasKey(e => e.Id);
    }
}

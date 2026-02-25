using Microsoft.EntityFrameworkCore;
using Ierahkwa.PostalMapsService.Domain;

namespace Ierahkwa.PostalMapsService.Infrastructure;

public class PostalMapsServiceDbContext : DbContext
{
    public PostalMapsServiceDbContext(DbContextOptions<PostalMapsServiceDbContext> options) : base(options) { }

    public DbSet<PostalPackage> PostalPackages => Set<PostalPackage>();
    public DbSet<Address> Addresss => Set<Address>();
    public DbSet<MapTile> MapTiles => Set<MapTile>();
    public DbSet<GeoPoint> GeoPoints => Set<GeoPoint>();
    public DbSet<DeliveryRoute> DeliveryRoutes => Set<DeliveryRoute>();}

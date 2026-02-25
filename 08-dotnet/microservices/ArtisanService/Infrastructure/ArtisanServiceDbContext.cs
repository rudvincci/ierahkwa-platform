using Microsoft.EntityFrameworkCore;
using Ierahkwa.ArtisanService.Domain;

namespace Ierahkwa.ArtisanService.Infrastructure;

public class ArtisanServiceDbContext : DbContext
{
    public ArtisanServiceDbContext(DbContextOptions<ArtisanServiceDbContext> options) : base(options) { }

    public DbSet<Artisan> Artisans => Set<Artisan>();
    public DbSet<Craft> Crafts => Set<Craft>();
    public DbSet<ArtisanOrder> ArtisanOrders => Set<ArtisanOrder>();
    public DbSet<MarketStall> MarketStalls => Set<MarketStall>();
    public DbSet<CulturalCertificate> CulturalCertificates => Set<CulturalCertificate>();}

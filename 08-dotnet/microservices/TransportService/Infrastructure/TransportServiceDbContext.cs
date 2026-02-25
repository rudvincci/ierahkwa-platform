using Microsoft.EntityFrameworkCore;
using Ierahkwa.TransportService.Domain;

namespace Ierahkwa.TransportService.Infrastructure;

public class TransportServiceDbContext : DbContext
{
    public TransportServiceDbContext(DbContextOptions<TransportServiceDbContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<TransitRoute> TransitRoutes => Set<TransitRoute>();
    public DbSet<FlightRecord> FlightRecords => Set<FlightRecord>();
    public DbSet<ShipRecord> ShipRecords => Set<ShipRecord>();
    public DbSet<TransportLicense> TransportLicenses => Set<TransportLicense>();}

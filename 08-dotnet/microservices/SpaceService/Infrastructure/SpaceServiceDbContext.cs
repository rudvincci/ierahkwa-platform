using Microsoft.EntityFrameworkCore;
using Ierahkwa.SpaceService.Domain;

namespace Ierahkwa.SpaceService.Infrastructure;

public class SpaceServiceDbContext : DbContext
{
    public SpaceServiceDbContext(DbContextOptions<SpaceServiceDbContext> options) : base(options) { }

    public DbSet<Satellite> Satellites => Set<Satellite>();
    public DbSet<LaunchMission> LaunchMissions => Set<LaunchMission>();
    public DbSet<GroundStation> GroundStations => Set<GroundStation>();
    public DbSet<OrbitData> OrbitData => Set<OrbitData>();
    public DbSet<EarthObservation> EarthObservations => Set<EarthObservation>();
    public DbSet<Telemetry> Telemetry => Set<Telemetry>();
}

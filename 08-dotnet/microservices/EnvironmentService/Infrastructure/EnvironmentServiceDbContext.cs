using EnvironmentService.Domain;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentService.Infrastructure;

public class EnvironmentServiceDbContext : DbContext
{
    public EnvironmentServiceDbContext(DbContextOptions<EnvironmentServiceDbContext> options) : base(options) { }

    public DbSet<EcoZone> EcoZones => Set<EcoZone>();
    public DbSet<WildlifeSpecies> WildlifeSpecies => Set<WildlifeSpecies>();
    public DbSet<GeologicalSurvey> GeologicalSurveys => Set<GeologicalSurvey>();
    public DbSet<WeatherStation> WeatherStations => Set<WeatherStation>();
    public DbSet<ReforestationProject> ReforestationProjects => Set<ReforestationProject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<EcoZone>().HasKey(e => e.Id);
        modelBuilder.Entity<WildlifeSpecies>().HasKey(e => e.Id);
        modelBuilder.Entity<GeologicalSurvey>().HasKey(e => e.Id);
        modelBuilder.Entity<WeatherStation>().HasKey(e => e.Id);
        modelBuilder.Entity<ReforestationProject>().HasKey(e => e.Id);
    }
}

using Microsoft.EntityFrameworkCore;
using Ierahkwa.EmergencyService.Domain;

namespace Ierahkwa.EmergencyService.Infrastructure;

public class EmergencyServiceDbContext : DbContext
{
    public EmergencyServiceDbContext(DbContextOptions<EmergencyServiceDbContext> options) : base(options) { }

    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<EmergencyUnit> EmergencyUnits => Set<EmergencyUnit>();
    public DbSet<FireStation> FireStations => Set<FireStation>();
    public DbSet<HumanitarianMission> HumanitarianMissions => Set<HumanitarianMission>();
    public DbSet<Alert> Alerts => Set<Alert>();
}

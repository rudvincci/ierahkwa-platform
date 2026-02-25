using Microsoft.EntityFrameworkCore;
using Ierahkwa.DroneService.Domain;

namespace Ierahkwa.DroneService.Infrastructure;

public class DroneServiceDbContext : DbContext
{
    public DroneServiceDbContext(DbContextOptions<DroneServiceDbContext> options) : base(options) { }

    public DbSet<Drone> Drones => Set<Drone>();
    public DbSet<FlightPlan> FlightPlans => Set<FlightPlan>();
    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<MaintenanceLog> MaintenanceLogs => Set<MaintenanceLog>();
    public DbSet<SensorData> SensorData => Set<SensorData>();
}

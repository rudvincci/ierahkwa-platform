using Microsoft.EntityFrameworkCore;
using Ierahkwa.IoTRoboticsService.Domain;

namespace Ierahkwa.IoTRoboticsService.Infrastructure;

public class IoTRoboticsServiceDbContext : DbContext
{
    public IoTRoboticsServiceDbContext(DbContextOptions<IoTRoboticsServiceDbContext> options) : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    public DbSet<DigitalTwin> DigitalTwins => Set<DigitalTwin>();
    public DbSet<RobotUnit> RobotUnits => Set<RobotUnit>();
    public DbSet<NeuralInterface> NeuralInterfaces => Set<NeuralInterface>();}

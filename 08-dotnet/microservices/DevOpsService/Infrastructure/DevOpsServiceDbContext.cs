using DevOpsService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevOpsService.Infrastructure;

public class DevOpsServiceDbContext : DbContext
{
    public DevOpsServiceDbContext(DbContextOptions<DevOpsServiceDbContext> options) : base(options) { }

    public DbSet<Pipeline> Pipelines => Set<Pipeline>();
    public DbSet<Deployment> Deployments => Set<Deployment>();
    public DbSet<Container> Containers => Set<Container>();
    public DbSet<ServiceMesh> ServiceMeshes => Set<ServiceMesh>();
    public DbSet<MonitoringAlert> MonitoringAlerts => Set<MonitoringAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Pipeline>().HasKey(e => e.Id);
        modelBuilder.Entity<Deployment>().HasKey(e => e.Id);
        modelBuilder.Entity<Container>().HasKey(e => e.Id);
        modelBuilder.Entity<ServiceMesh>().HasKey(e => e.Id);
        modelBuilder.Entity<MonitoringAlert>().HasKey(e => e.Id);
    }
}

using Microsoft.EntityFrameworkCore;
using Ierahkwa.TelecomService.Domain;

namespace Ierahkwa.TelecomService.Infrastructure;

public class TelecomServiceDbContext : DbContext
{
    public TelecomServiceDbContext(DbContextOptions<TelecomServiceDbContext> options) : base(options) { }

    public DbSet<Subscriber> Subscribers => Set<Subscriber>();
    public DbSet<CallRecord> CallRecords => Set<CallRecord>();
    public DbSet<NetworkNode> NetworkNodes => Set<NetworkNode>();
    public DbSet<MeshDevice> MeshDevices => Set<MeshDevice>();
    public DbSet<Bandwidth> Bandwidths => Set<Bandwidth>();
    public DbSet<ServicePlan> ServicePlans => Set<ServicePlan>();
}

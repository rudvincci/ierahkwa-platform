using Microsoft.EntityFrameworkCore;
using Ierahkwa.NetworkService.Domain;

namespace Ierahkwa.NetworkService.Infrastructure;

public class NetworkServiceDbContext : DbContext
{
    public NetworkServiceDbContext(DbContextOptions<NetworkServiceDbContext> options) : base(options) { }

    public DbSet<NetworkNode> NetworkNodes => Set<NetworkNode>();
    public DbSet<Connection> Connections => Set<Connection>();
    public DbSet<BandwidthAllocation> BandwidthAllocations => Set<BandwidthAllocation>();
    public DbSet<CDNEdge> CDNEdges => Set<CDNEdge>();
    public DbSet<TrafficRule> TrafficRules => Set<TrafficRule>();}

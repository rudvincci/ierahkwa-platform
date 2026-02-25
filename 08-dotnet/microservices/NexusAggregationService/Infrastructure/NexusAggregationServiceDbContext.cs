using Microsoft.EntityFrameworkCore;
using Ierahkwa.NexusAggregationService.Domain;

namespace Ierahkwa.NexusAggregationService.Infrastructure;

public class NexusAggregationServiceDbContext : DbContext
{
    public NexusAggregationServiceDbContext(DbContextOptions<NexusAggregationServiceDbContext> options) : base(options) { }

    public DbSet<NexusDomain> NexusDomains => Set<NexusDomain>();
    public DbSet<PlatformEntry> PlatformEntrys => Set<PlatformEntry>();
    public DbSet<DashboardWidget> DashboardWidgets => Set<DashboardWidget>();
    public DbSet<SystemMetric> SystemMetrics => Set<SystemMetric>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();}

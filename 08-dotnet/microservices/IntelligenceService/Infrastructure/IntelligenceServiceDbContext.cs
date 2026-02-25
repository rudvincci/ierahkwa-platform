using Microsoft.EntityFrameworkCore;
using Ierahkwa.IntelligenceService.Domain;

namespace Ierahkwa.IntelligenceService.Infrastructure;

public class IntelligenceServiceDbContext : DbContext
{
    public IntelligenceServiceDbContext(DbContextOptions<IntelligenceServiceDbContext> options) : base(options) { }

    public DbSet<IntelReport> IntelReports => Set<IntelReport>();
    public DbSet<Surveillance> Surveillances => Set<Surveillance>();
    public DbSet<BorderCrossing> BorderCrossings => Set<BorderCrossing>();
    public DbSet<ThreatAssessment> ThreatAssessments => Set<ThreatAssessment>();
    public DbSet<Watchlist> Watchlists => Set<Watchlist>();
}

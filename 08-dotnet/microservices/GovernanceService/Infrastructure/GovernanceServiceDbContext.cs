using Microsoft.EntityFrameworkCore;
using Ierahkwa.GovernanceService.Domain;

namespace Ierahkwa.GovernanceService.Infrastructure;

public class GovernanceServiceDbContext : DbContext
{
    public GovernanceServiceDbContext(DbContextOptions<GovernanceServiceDbContext> options) : base(options) { }

    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Law> Laws => Set<Law>();
    public DbSet<Amendment> Amendments => Set<Amendment>();
    public DbSet<ParliamentSession> ParliamentSessions => Set<ParliamentSession>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Regulation> Regulations => Set<Regulation>();
}

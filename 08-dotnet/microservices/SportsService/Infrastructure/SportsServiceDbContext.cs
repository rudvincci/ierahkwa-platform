using Microsoft.EntityFrameworkCore;
using Ierahkwa.SportsService.Domain;

namespace Ierahkwa.SportsService.Infrastructure;

public class SportsServiceDbContext : DbContext
{
    public SportsServiceDbContext(DbContextOptions<SportsServiceDbContext> options) : base(options) { }

    public DbSet<League> Leagues => Set<League>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Athlete> Athletes => Set<Athlete>();
    public DbSet<Match> Matchs => Set<Match>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Standing> Standings => Set<Standing>();}

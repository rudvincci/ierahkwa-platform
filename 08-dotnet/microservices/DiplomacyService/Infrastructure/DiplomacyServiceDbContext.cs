using Microsoft.EntityFrameworkCore;
using Ierahkwa.DiplomacyService.Domain;

namespace Ierahkwa.DiplomacyService.Infrastructure;

public class DiplomacyServiceDbContext : DbContext
{
    public DiplomacyServiceDbContext(DbContextOptions<DiplomacyServiceDbContext> options) : base(options) { }

    public DbSet<Treaty> Treaties => Set<Treaty>();
    public DbSet<Embassy> Embassies => Set<Embassy>();
    public DbSet<CustomsDeclaration> CustomsDeclarations => Set<CustomsDeclaration>();
    public DbSet<DiplomaticMission> DiplomaticMissions => Set<DiplomaticMission>();
    public DbSet<TradeAgreement> TradeAgreements => Set<TradeAgreement>();
}

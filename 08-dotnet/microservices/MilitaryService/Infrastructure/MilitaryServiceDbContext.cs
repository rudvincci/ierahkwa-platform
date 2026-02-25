using Microsoft.EntityFrameworkCore;
using Ierahkwa.MilitaryService.Domain;

namespace Ierahkwa.MilitaryService.Infrastructure;

public class MilitaryServiceDbContext : DbContext
{
    public MilitaryServiceDbContext(DbContextOptions<MilitaryServiceDbContext> options) : base(options) { }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Personnel> Personnel => Set<Personnel>();
    public DbSet<MilitaryAsset> MilitaryAssets => Set<MilitaryAsset>();
    public DbSet<LogisticsOrder> LogisticsOrders => Set<LogisticsOrder>();
    public DbSet<FieldHospitalRecord> FieldHospitalRecords => Set<FieldHospitalRecord>();
}

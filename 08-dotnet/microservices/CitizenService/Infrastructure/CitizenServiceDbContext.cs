using Microsoft.EntityFrameworkCore;
using Ierahkwa.CitizenService.Domain;

namespace Ierahkwa.CitizenService.Infrastructure;

public class CitizenServiceDbContext : DbContext
{
    public CitizenServiceDbContext(DbContextOptions<CitizenServiceDbContext> options) : base(options) { }

    public DbSet<CitizenRecord> CitizenRecords => Set<CitizenRecord>();
    public DbSet<Passport> Passports => Set<Passport>();
    public DbSet<ImmigrationVisa> ImmigrationVisas => Set<ImmigrationVisa>();
    public DbSet<CensusEntry> CensusEntries => Set<CensusEntry>();
    public DbSet<BirthCertificate> BirthCertificates => Set<BirthCertificate>();
}

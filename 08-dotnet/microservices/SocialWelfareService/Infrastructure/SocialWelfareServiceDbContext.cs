using Microsoft.EntityFrameworkCore;
using Ierahkwa.SocialWelfareService.Domain;

namespace Ierahkwa.SocialWelfareService.Infrastructure;

public class SocialWelfareServiceDbContext : DbContext
{
    public SocialWelfareServiceDbContext(DbContextOptions<SocialWelfareServiceDbContext> options) : base(options) { }

    public DbSet<WelfareCase> WelfareCases => Set<WelfareCase>();
    public DbSet<FamilyRecord> FamilyRecords => Set<FamilyRecord>();
    public DbSet<VeteranBenefit> VeteranBenefits => Set<VeteranBenefit>();
    public DbSet<UnemploymentClaim> UnemploymentClaims => Set<UnemploymentClaim>();
    public DbSet<DisabilityRecord> DisabilityRecords => Set<DisabilityRecord>();
}

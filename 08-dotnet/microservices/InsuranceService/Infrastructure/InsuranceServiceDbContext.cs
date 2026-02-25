using Microsoft.EntityFrameworkCore;
using Ierahkwa.InsuranceService.Domain;

namespace Ierahkwa.InsuranceService.Infrastructure;

public class InsuranceServiceDbContext : DbContext
{
    public InsuranceServiceDbContext(DbContextOptions<InsuranceServiceDbContext> options) : base(options) { }

    public DbSet<Policy> Policys => Set<Policy>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<PensionAccount> PensionAccounts => Set<PensionAccount>();
    public DbSet<BasicIncomeRecipient> BasicIncomeRecipients => Set<BasicIncomeRecipient>();
    public DbSet<Beneficiary> Beneficiarys => Set<Beneficiary>();}

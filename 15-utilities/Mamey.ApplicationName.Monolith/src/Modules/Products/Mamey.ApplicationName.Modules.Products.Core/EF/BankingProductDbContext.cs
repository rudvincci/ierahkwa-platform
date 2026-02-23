using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Mamey.ApplicationName.Modules.Products.Core.EF;

internal class BankingProductDbContext : DbContext
{
    /// <summary>
    /// Override OnModelCreating to configure entity mappings.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public BankingProductDbContext(DbContextOptions<BankingProductDbContext> options) : base(options)
    {
    }

    public DbSet<InboxMessage> Inbox { get; set; }
    public DbSet<OutboxMessage> Outbox { get; set; }
    // DbSets for all entities in the banking product domain
    /// <summary>
    /// DbSet for Banking Products.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// DbSet for Interest Rates.
    /// </summary>
    public DbSet<InterestRate> InterestRates { get; set; }

    /// <summary>
    /// DbSet for Fees.
    /// </summary>
    public DbSet<Fee> Fees { get; set; }

    /// <summary>
    /// DbSet for Limits.
    /// </summary>
    public DbSet<Limits> Limits { get; set; }

    /// <summary>
    /// DbSet for Benefits.
    /// </summary>
    public DbSet<Benefit> Benefits { get; set; }

    /// <summary>
    /// DbSet for Applicable Taxes.
    /// </summary>
    public DbSet<ApplicableTax> ApplicableTaxes { get; set; }

    /// <summary>
    /// DbSet for Required Documents.
    /// </summary>
    public DbSet<RequiredDocument> RequiredDocuments { get; set; }

    /// <summary>
    /// DbSet for Eligibility Criteria.
    /// </summary>
    public DbSet<EligibilityCriteria> EligibilityCriterias { get; set; }

    /// <summary>
    /// Override OnModelCreating to configure entity mappings and relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entities.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("banking-products-module");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
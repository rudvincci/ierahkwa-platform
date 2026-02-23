using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.ApplicationName.Modules.Products.Core.EF.Configurations;

internal class EligibilityCriteriaEntityTypeConfiguration : IEntityTypeConfiguration<EligibilityCriteria>
{
    public void Configure(EntityTypeBuilder<EligibilityCriteria> builder)
    {
        builder.HasKey(ec => ec.Id); // Primary key
        builder.Property(ec => ec.MinAge).IsRequired(false); // Minimum age is optional
        builder.Property(ec => ec.MaxAge).IsRequired(false); // Maximum age is optional
        builder.Property(ec => ec.MinimumIncome)
            .HasColumnType("decimal(18,4)"); // Minimum income as a decimal
        builder.Property(ec => ec.Geography)
            .HasMaxLength(100); // Geographies with maximum length
        // builder.OwnsMany(c => c.Geographies, currencies =>
        // {
        //     currencies.WithOwner().HasForeignKey("ExampleEntityId");
        //     currencies.Property(currency => currency)
        //         .HasColumnName("Currencies")
        //         .HasMaxLength(100)
        //         .IsRequired();
        //     currencies.ToTable("ProductCurrencies");
        // });
        
        builder.Property(ec => ec.OtherCriteria)
            .HasMaxLength(500); // Other criteria is optional with max length
        builder.HasOne(ec => ec.Product)
            .WithOne(bp => bp.EligibilityCriteria)
            .HasForeignKey<EligibilityCriteria>(ec => ec.BankingProductId) // One-to-one relationship
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete
    }
}
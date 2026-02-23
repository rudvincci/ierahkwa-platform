
using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Mamey.ApplicationName.Modules.Products.Core.EF.Configurations;

internal class BankingProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Primary Key
        builder.HasKey(b => b.Id);
        

        // Properties
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100); // Product Name is required with a max length

        builder.Property(b => b.Currency)
            .IsRequired()
            .HasMaxLength(3); // Currencies code (ISO standard)

        builder.Property(b => b.Status)
            .IsRequired()
            .HasMaxLength(50); // Status of the product (Active, Inactive, etc.)

        builder.Property(b => b.CreatedDate)
            .IsRequired(); // Auto-set creation date

        builder.Property(b => b.ModifiedDate)
            .IsRequired(); // Auto-set modified date

        builder.Property(b => b.TermsAndConditions)
            .HasMaxLength(2000); // Terms and Conditions (optional, but large)

        // Enum Conversions
        builder.Property(b => b.ProductType)
            .HasConversion<string>(); // Enum to string conversion for ProductType

        builder.Property(b => b.AccountCategory)
            .HasConversion<string>(); // Enum to string conversion for AccountCategory

        // Relationships
        
        // InterestRate - One-to-One
        builder.HasOne(b => b.InterestRate)
            .WithOne(ec => ec.Product)
            .HasForeignKey<InterestRate>(ec => ec.BankingProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Limits - Owned Type
        // builder.HasOne(b => b.Limits)
        //     .WithOne(ec => ec.Product)
        //     .HasForeignKey<Limits>(ec => ec.BankingProductId)
        //     .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(b => b.Limits)
            .WithOne(ec => ec.Product)
            .HasForeignKey<Limits>(ec => ec.BankingProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fees - One-to-Many
        builder.HasMany(b => b.Fees)
            .WithOne()
            .HasForeignKey("BankingProductId")
            .OnDelete(DeleteBehavior.Cascade);

        // Benefits - One-to-Many
        builder.HasMany(b => b.Benefits)
            .WithOne(bn => bn.Product)
            .HasForeignKey(bn => bn.BankingProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ApplicableTaxes - One-to-Many
        builder.HasMany(b => b.ApplicableTaxes)
            .WithOne(t => t.Product)
            .HasForeignKey(t => t.BankingProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // RequiredDocuments - One-to-Many
        builder.HasMany(b => b.RequiredDocuments)
            .WithOne(d => d.Product)
            .HasForeignKey(d => d.BankingProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // EligibilityCriteria - One-to-One
        builder.HasOne(b => b.EligibilityCriteria)
            .WithOne(ec => ec.Product)
            .HasForeignKey<EligibilityCriteria>(ec => ec.BankingProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexing
        builder.HasIndex(b => b.Name)
            .IsUnique(); // Ensure product names are uniqu
    }
}
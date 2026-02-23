using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.ApplicationName.Modules.Products.Core.EF.Configurations;

internal class InterestRateEntityConfiguration : IEntityTypeConfiguration<InterestRate>
{
    public void Configure(EntityTypeBuilder<InterestRate> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Rate).IsRequired().HasColumnType("decimal(18,4)");
        builder.Property(i => i.Type).IsRequired().HasConversion<string>();
        builder.Property(i => i.CompoundingFrequency).HasMaxLength(50);
        builder.HasOne(i => i.Product)                // InterestRate has one Product
            .WithOne(p => p.InterestRate)             // Product has one InterestRate
            .HasForeignKey<InterestRate>(i => i.BankingProductId) // Foreign key is in InterestRate
            .OnDelete(DeleteBehavior.Cascade);        // Cascade delete
        
    }
}

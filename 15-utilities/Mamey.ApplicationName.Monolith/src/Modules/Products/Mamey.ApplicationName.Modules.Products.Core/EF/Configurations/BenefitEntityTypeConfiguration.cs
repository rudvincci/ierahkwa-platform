using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.ApplicationName.Modules.Products.Core.EF.Configurations;

internal class BenefitEntityTypeConfiguration: IEntityTypeConfiguration<Benefit>
{
    public void Configure(EntityTypeBuilder<Benefit> builder)
    {
        builder.HasKey(b => b.Id); // Primary key
        builder.Property(b => b.BenefitType)
            .IsRequired()
            .HasMaxLength(100); // Benefit type is required
        builder.Property(b => b.Description)
            .HasMaxLength(500); // Optional description
        builder.HasOne(b => b.Product)
            .WithMany(bp => bp.Benefits)
            .HasForeignKey(b => b.BankingProductId) // Foreign key relationship
            .OnDelete(DeleteBehavior.Cascade); 
    }
}

using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Mamey.ApplicationName.Modules.Products.Core.EF.Configurations;

internal class ApplicableTaxEntityTypeConfiguration : IEntityTypeConfiguration<ApplicableTax>
{
    public void Configure(EntityTypeBuilder<ApplicableTax> builder)
    {
        builder.HasKey(t => t.Id); // Primary key
        builder.Property(t => t.TaxName)
            .IsRequired()
            .HasMaxLength(100); // Tax name is required
        builder.Property(t => t.TaxRate)
            .IsRequired()
            .HasColumnType("decimal(18,4)"); // Tax rate is decimal with precision
        builder.HasOne(t => t.Product)
            .WithMany(bp => bp.ApplicableTaxes)
            .HasForeignKey(t => t.BankingProductId) // Foreign key relationship
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete
    }
}
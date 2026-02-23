using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;namespace Mamey.ApplicationName.Modules.Products.Core.EF.Configurations;

internal class RequiredDocumentEntityTypeConfiguration : IEntityTypeConfiguration<RequiredDocument>
{
    public void Configure(EntityTypeBuilder<RequiredDocument> builder)
    {
        builder.HasKey(d => d.Id); // Primary key
        builder.Property(d => d.DocumentName)
            .IsRequired()
            .HasMaxLength(100); // Document name is required
        builder.HasOne(d => d.Product)
            .WithMany(bp => bp.RequiredDocuments)
            .HasForeignKey(d => d.BankingProductId) // Foreign key relationship
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete
    }
}
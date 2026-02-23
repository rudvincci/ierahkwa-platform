using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Mamey;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Mamey.ApplicationName.Modules.Products.Core.EF.Configurations;

internal class FeeEntityConfiguration: IEntityTypeConfiguration<Fee>
{
    public void Configure(EntityTypeBuilder<Fee> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.FeeType).IsRequired()
            .HasMaxLength(0)
            .HasConversion<string>(x=> x.ToString(), 
                x=> x.ToEnum<FeeType>());
        
        builder.Property(f => f.Amount).IsRequired().HasColumnType("decimal(18,4)");
        builder.Property(f => f.Frequency).HasMaxLength(50).HasConversion<string>();
        builder.HasOne(t => t.Product)
            .WithMany(bp => bp.Fees)
            .HasForeignKey(t => t.BankingProductId) // Foreign key relationship
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete
    }
}
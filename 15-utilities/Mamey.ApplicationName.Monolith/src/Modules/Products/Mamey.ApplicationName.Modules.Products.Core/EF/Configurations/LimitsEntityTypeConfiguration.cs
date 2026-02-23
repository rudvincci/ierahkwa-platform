using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.ApplicationName.Modules.Products.Core.EF.Configurations;

internal class LimitsEntityTypeConfiguration: IEntityTypeConfiguration<Limits>
{
    public void Configure(EntityTypeBuilder<Limits> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.MinimumBalance).HasColumnType("decimal(18,4)");
        builder.Property(l => l.MaximumBalance).HasColumnType("decimal(18,4)");
        builder.Property(l => l.DailyTransactionLimit).HasColumnType("decimal(18,4)");
        builder.Property(l => l.WithdrawalLimit).HasColumnType("decimal(18,4)");
        
        builder.HasOne(i => i.Product)
            .WithOne(p => p.Limits)
            .HasForeignKey<Product>(i => i.LimitsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
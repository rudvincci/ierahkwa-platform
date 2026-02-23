using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Passports.Core.EF.Configuration;

internal class PassportRowConfiguration : IEntityTypeConfiguration<PassportRow>
{
    public void Configure(EntityTypeBuilder<PassportRow> builder)
    {
        builder.ToTable("passports", "passports");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.CitizenId).IsRequired();
        builder.Property(e => e.PassportNumber).IsRequired().HasMaxLength(50);
        
        builder.HasIndex(e => e.PassportNumber)
            .IsUnique()
            .HasDatabaseName("IX_passports_passport_number");
        
        builder.HasIndex(e => e.CitizenId)
            .HasDatabaseName("IX_passports_citizen_id");
        
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_passports_tenant_id");
    }
}

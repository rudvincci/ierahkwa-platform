using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.TravelIdentities.Core.EF.Configuration;

internal class TravelIdentityRowConfiguration : IEntityTypeConfiguration<TravelIdentityRow>
{
    public void Configure(EntityTypeBuilder<TravelIdentityRow> builder)
    {
        builder.ToTable("travel_identities", "travel_identities");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.CitizenId).IsRequired();
        builder.Property(e => e.TravelIdentityNumber).IsRequired().HasMaxLength(50);
        
        builder.HasIndex(e => e.TravelIdentityNumber)
            .IsUnique()
            .HasDatabaseName("IX_travel_identities_travel_identity_number");
        
        builder.HasIndex(e => e.CitizenId)
            .HasDatabaseName("IX_travel_identities_citizen_id");
        
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_travel_identities_tenant_id");
    }
}

using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Citizens.Core.EF.Configuration;

internal class CitizenshipStatusHistoryRowConfiguration : IEntityTypeConfiguration<CitizenshipStatusHistoryRow>
{
    public void Configure(EntityTypeBuilder<CitizenshipStatusHistoryRow> builder)
    {
       
        builder.HasKey(e => new { e.CitizenId, e.ChangedAt });
        builder.Property(e => e.Status).HasConversion<int>();
        
        builder.HasOne<CitizenRow>()
            .WithMany(c => c.StatusHistory)
            .HasForeignKey(e => e.CitizenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.ApplicationName.Modules.Notifications.Core.EF.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(c=> c.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, 
                x => new UserId(x));
        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, 
                x => new Email(x));
        
        builder.OwnsOne(e => e.Name, name =>
        {
            name.Property(n => n.FirstName).HasColumnName("FirstName").HasMaxLength(50);
            name.Property(n => n.LastName).HasColumnName("LastName").HasMaxLength(50);
            name.Property(n => n.MiddleName).HasColumnName("MiddleName").HasMaxLength(50);
            name.Property(n => n.Nickname).HasColumnName("Nickname").HasMaxLength(50);
        });
    }
}
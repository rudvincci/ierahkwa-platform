using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Notifications.Core.EF.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(c=> c.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, 
                x => new UserId(x))
            .HasColumnName("id");
        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, 
                x => new Email(x))
            .HasColumnName("email")
            .HasMaxLength(255);
        
        builder.OwnsOne(e => e.Name, name =>
        {
            // Share PK/FK column "id" with owner
            name.WithOwner().HasForeignKey("id");
            name.HasKey("id");
            name.Property<Guid>("id").HasColumnName("id");
            
            name.Property(n => n.FirstName).HasColumnName("first_name").HasMaxLength(50).IsRequired();
            name.Property(n => n.LastName).HasColumnName("last_name").HasMaxLength(50).IsRequired();
            name.Property(n => n.MiddleName).HasColumnName("middle_name").HasMaxLength(50);
            name.Property(n => n.Nickname).HasColumnName("nickname").HasMaxLength(50);
        });
    }
}
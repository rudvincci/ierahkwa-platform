using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Notifications.Core.EF.Configurations
{
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(c=> c.Id);
            builder.Property(x => x.Id)
                .HasConversion(x => x.Value, 
                    x => new NotificationId(x));

            builder.Property(c => c.UserId)
                .HasConversion(c => c!.Value, 
                    c => new UserId(c));
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(c => c.Message)
                .IsRequired()
                .HasMaxLength(300);
            
            builder.Property(c => c.Category)
                .IsRequired()
                .HasConversion(c=> c.ToString(), 
                    x => x.ToEnum<NotificationCategory>() );
            
            builder.Property(c => c.Icon)
                .HasMaxLength(30);
            builder.Ignore(c => c.Events);
            builder.Property(c => c.Version)
                .IsConcurrencyToken();
        }
    }
}
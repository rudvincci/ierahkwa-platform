using Mamey.ApplicationName.Modules.Customers.Core.Domain.Entities;
using Mamey.ApplicationName.Modules.Customers.Core.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Bank.Modules.Customers.Infrastructure.EF.Configurations;
internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email).IsRequired()
            .HasMaxLength(100)
            .HasConversion(x => x.Value, x => new Email(x));
            
        builder.HasIndex(x => x.Name).IsUnique();
        
        builder.OwnsOne(e => e.Name, name =>
        {
            name.Property(n => n.FirstName).HasColumnName("first_name").HasMaxLength(50);
            name.Property(n => n.LastName).HasColumnName("last_name").HasMaxLength(50);
            name.Property(n => n.MiddleName).HasColumnName("middle_name").HasMaxLength(50);
            name.Property(n => n.Nickname).HasColumnName("nickname").HasMaxLength(50);
        });

        builder.Property(x => x.FullName).HasMaxLength(100)
            .HasConversion(x => x.Value, x => new FullName(x, 200));
        
        builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(n => n.FirmName).HasColumnName("first_name").HasMaxLength(100);
            address.Property(n => n.Line).HasColumnName("line").HasMaxLength(100);
            address.Property(n => n.Line2).HasColumnName("line2").HasMaxLength(100);
            address.Property(n => n.Line3).HasColumnName("line3").HasMaxLength(100);
            address.Property(n => n.Urbanization).HasColumnName("urbanization").HasMaxLength(100);
            address.Property(n => n.City).HasColumnName("city").HasMaxLength(50);
            address.Property(n => n.State).HasColumnName("state").HasMaxLength(50);
            address.Property(n => n.Country).HasColumnName("country").HasMaxLength(2);
            address.Property(n => n.Province).HasColumnName("province").HasMaxLength(50);
            address.Property(n => n.PostalCode).HasColumnName("postalCode").HasMaxLength(50);
            address.Property(n => n.Zip5).HasColumnName("zip5").HasMaxLength(5);
            address.Property(n => n.Zip4).HasColumnName("zip4").HasMaxLength(4);
            // Ignore the computed field
            address.Ignore(a => a.ZipCode);
        });
            
        builder.Property(x => x.Identity).HasMaxLength(40)
            .HasConversion(x => x.ToString(), x => Identity.From(x));
            
        builder.Property(x => x.Nationality).HasMaxLength(2)
            .HasConversion(x => x.Value, x => new Nationality(x));
            
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.Property(c => c.Version).IsConcurrencyToken().IsRequired();
    }
}


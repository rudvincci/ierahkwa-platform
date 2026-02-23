using System;
using System.Text.Json;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Mamey.EnumExtensions;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

public class IdentityConfiguration : IEntityTypeConfiguration<Identity>
{
    public IdentityConfiguration()
    {
    }

    void IEntityTypeConfiguration<Identity>.Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.ToTable("identity");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .HasConversion(c => c.Value, c => new IdentityId(c))
            .IsRequired();

        // Store Status enum as string instead of int
        builder.Property(c => c.Status)
            .HasConversion(
                v => v.ToString(),
                v => v.ToEnum<IdentityStatus>())
            .HasMaxLength(50)
            .IsRequired();

        // Configure Name value object (owned entity)
        builder.OwnsOne(c => c.Name, na =>
        {
            // Share PK/FK column "id" with owner
            na.WithOwner().HasForeignKey("id");
            na.HasKey("id");
            na.Property<Guid>("id").HasColumnName("id");
            
            na.Property(n => n.FirstName).HasColumnName("name_first_name").HasMaxLength(100).IsRequired();
            na.Property(n => n.MiddleName).HasColumnName("name_middle_name").HasMaxLength(100);
            na.Property(n => n.LastName).HasColumnName("name_last_name").HasMaxLength(100).IsRequired();
            na.Property(n => n.Nickname).HasColumnName("name_nickname").HasMaxLength(100);
        });

        // Configure PersonalDetails value object (owned entity)
        builder.OwnsOne(c => c.PersonalDetails, pd =>
        {
            // Share PK/FK column "id" with owner
            pd.WithOwner().HasForeignKey("id");
            pd.HasKey("id");
            pd.Property<Guid>("id").HasColumnName("id");
            
            pd.Property(p => p.DateOfBirth).HasColumnName("personal_details_date_of_birth");
            pd.Property(p => p.PlaceOfBirth).HasColumnName("personal_details_place_of_birth").HasMaxLength(200);
            pd.Property(p => p.Gender).HasColumnName("personal_details_gender").HasMaxLength(50);
            pd.Property(p => p.ClanAffiliation).HasColumnName("personal_details_clan_affiliation").HasMaxLength(200);
        });

        // Configure ContactInformation value object (owned entity)
        builder.OwnsOne(c => c.ContactInformation, ci =>
        {
            // Share PK/FK column "id" with owner
            ci.WithOwner().HasForeignKey("id");
            ci.HasKey("id");
            ci.Property<Guid>("id").HasColumnName("id");
            
            // Email (Email type from Mamey.Types) - convert to string
            ci.Property(c => c.Email)
                .HasConversion(
                    email => email != null ? email.Value : null,
                    value => value != null ? new Email(value) : null)
                .HasColumnName("contact_email")
                .HasMaxLength(200)
                .IsRequired(false);
            
            // Address (Address type from Mamey.Types) - owned entity
            ci.OwnsOne(c => c.Address, a =>
            {
                // Share PK/FK column "id" with owner (ContactInformation shares Identity's id)
                a.WithOwner().HasForeignKey("id");
                a.HasKey("id");
                a.Property<Guid>("id").HasColumnName("id");
                
                a.Property(addr => addr.FirmName).HasColumnName("contact_address_firm_name").HasMaxLength(200);
                a.Property(addr => addr.Line).HasColumnName("contact_address_line").HasMaxLength(200).IsRequired(false);
                a.Property(addr => addr.Line2).HasColumnName("contact_address_line2").HasMaxLength(200);
                a.Property(addr => addr.Line3).HasColumnName("contact_address_line3").HasMaxLength(200);
                a.Property(addr => addr.Urbanization).HasColumnName("contact_address_urbanization").HasMaxLength(200);
                a.Property(addr => addr.City).HasColumnName("contact_address_city").HasMaxLength(100).IsRequired(false);
                a.Property(addr => addr.State).HasColumnName("contact_address_state").HasMaxLength(100).IsRequired(false);
                a.Property(addr => addr.Zip5).HasColumnName("contact_address_zip5").HasMaxLength(5);
                a.Property(addr => addr.Zip4).HasColumnName("contact_address_zip4").HasMaxLength(4);
                a.Property(addr => addr.PostalCode).HasColumnName("contact_address_postal_code").HasMaxLength(20);
                a.Property(addr => addr.Country).HasColumnName("contact_address_country").HasMaxLength(100).IsRequired(false);
                a.Property(addr => addr.Province).HasColumnName("contact_address_province").HasMaxLength(100);
                // Store AddressType enum as string
                a.Property(addr => addr.Type)
                    .HasConversion(
                        v => v.ToString(),
                        v => v.ToEnum<Address.AddressType>())
                    .HasColumnName("contact_address_type")
                    .HasMaxLength(50);
            });
            
            // PhoneNumbers is a collection - store as JSONB
            // Note: PhoneNumbers is a read-only property backed by a private field
            // For now, we'll ignore this and handle it separately if needed
            // Or store as JSONB with proper converter using backing field
            ci.Ignore(c => c.PhoneNumbers);
        });

        // Configure BiometricData value object (owned entity) - store BiometricType enum as string
        builder.OwnsOne(c => c.BiometricData, bd =>
        {
            // Share PK/FK column "id" with owner
            bd.WithOwner().HasForeignKey("id");
            bd.HasKey("id");
            bd.Property<Guid>("id").HasColumnName("id");
            
            // Store BiometricType enum as string
            bd.Property(b => b.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => v.ToEnum<BiometricType>())
                .HasColumnName("biometric_type")
                .HasMaxLength(50)
                .IsRequired();
            
            bd.Property(b => b.EncryptedTemplate)
                .HasColumnName("biometric_encrypted_template")
                .IsRequired();
            
            bd.Property(b => b.Hash)
                .HasColumnName("biometric_hash")
                .HasMaxLength(256)
                .IsRequired();
            
            bd.Property(b => b.CapturedAt)
                .HasColumnName("biometric_captured_at")
                .IsRequired();
        });

        // Note: Tags property removed from Identity entity per TDD
        // Tags are now stored in Metadata dictionary if needed

        // Configure Metadata dictionary as JSON
        builder.Property(c => c.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
            )
            .HasColumnType("jsonb");

        // Configure authentication properties
        // Note: PasswordHash is NOT marked with [Hashed] attribute, so it's stored as-is (already hashed)
        builder.Property(c => c.Username)
            .HasColumnName("username")
            .HasMaxLength(200);
        
        builder.Property(c => c.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(256); // SHA-512 hex string is 128 characters, but allow extra space

        builder.Ignore(c => c.Events);
        
        // Configure Version property as concurrency token for optimistic locking
        // Note: Version is managed by AggregateRoot<T> methods (IncrementVersion, AddEvent)
        // EF Core should only track it as a concurrency token, not auto-generate it
        builder.Property(c => c.Version)
            .IsConcurrencyToken()
            .HasColumnName("version")
            .IsRequired();
        
        // Configure external authentication properties
        builder.Property(c => c.AzureUserId)
            .HasColumnName("azure_user_id")
            .HasMaxLength(200);

        builder.Property(c => c.ServiceId)
            .HasColumnName("service_id")
            .HasMaxLength(200);

        // Indexes
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.Zone);
        builder.HasIndex(c => c.AzureUserId).IsUnique().HasFilter("[azure_user_id] IS NOT NULL");
        builder.HasIndex(c => c.ServiceId).IsUnique().HasFilter("[service_id] IS NOT NULL");
    }
}


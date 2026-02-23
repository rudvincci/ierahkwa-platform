using System.Text.Json;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Mamey.EnumExtensions;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF.Configuration;

internal class CitizenshipApplicationsConfiguration : IEntityTypeConfiguration<CitizenshipApplication>
{
    public void Configure(EntityTypeBuilder<CitizenshipApplication> builder)
    {

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasConversion(
                v => v.Value,
                v => new AppId(v))
            .ValueGeneratedNever();

        // Value Object Conversions
        builder.Property(e => e.TenantId)
            .HasConversion(
                v => v.Value,
                v => new TenantId(v))
            .IsRequired();

        builder.Property(e => e.ApplicationNumber)
            .HasConversion(
                v => v.Value,
                v => new ApplicationNumber(v))
            .IsRequired()
            .HasMaxLength(50);
        
        builder.OwnsOne(c => c.ApplicantName, na =>
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

        // Configure Email value object (simple conversion)
        builder.Property(e => e.Email)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? new Email(v) : null)
            .HasColumnName("email")
            .HasMaxLength(255);

        // Configure Phone value object (simple conversion)
        builder.OwnsOne(e => e.Phone, a =>
        {
            a.WithOwner().HasForeignKey("id");
            a.HasKey("id");
            a.Property<Guid>("id").HasColumnName("id");
            
            a.Property(c=> c.CountryCode).HasColumnName("phone_country_code");
            a.Property(c=> c.Number).HasColumnName("phone_number");
            a.Property(c=> c.Extension).HasColumnName("phone_extension");
            a.Property(c=> c.IsDefault).HasColumnName("phone_is_default");
            a.Property(addr => addr.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => v.ToEnum<Phone.PhoneType>())
                .HasColumnName("phone_type")
                .HasMaxLength(50);
        });
        // Configure Address value object (owned entity)
        builder.OwnsOne(e => e.Address, a =>
        {
            // Share PK/FK column "id" with owner
            a.WithOwner().HasForeignKey("id");
            a.HasKey("id");
            a.Property<Guid>("id").HasColumnName("id");
            
            a.Property(addr => addr.FirmName).HasColumnName("address_firm_name").HasMaxLength(200);
            a.Property(addr => addr.Line).HasColumnName("address_line").HasMaxLength(200).IsRequired(false);
            a.Property(addr => addr.Line2).HasColumnName("address_line2").HasMaxLength(200);
            a.Property(addr => addr.Line3).HasColumnName("address_line3").HasMaxLength(200);
            a.Property(addr => addr.Urbanization).HasColumnName("address_urbanization").HasMaxLength(200);
            a.Property(addr => addr.City).HasColumnName("address_city").HasMaxLength(100).IsRequired(false);
            a.Property(addr => addr.State).HasColumnName("address_state").HasMaxLength(100).IsRequired(false);
            a.Property(addr => addr.Zip5).HasColumnName("address_zip5").HasMaxLength(5);
            a.Property(addr => addr.Zip4).HasColumnName("address_zip4").HasMaxLength(4);
            a.Property(addr => addr.PostalCode).HasColumnName("address_postal_code").HasMaxLength(20);
            a.Property(addr => addr.Country).HasColumnName("address_country").HasMaxLength(100).IsRequired(false);
            a.Property(addr => addr.Province).HasColumnName("address_province").HasMaxLength(100);
            // Store AddressType enum as string
            a.Property(addr => addr.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => v.ToEnum<Address.AddressType>())
                .HasColumnName("address_type")
                .HasMaxLength(50);
            a.Property(addr => addr.IsDefault).HasColumnName("address_is_default");
            // Ignore computed property
            a.Ignore(addr => addr.Street);
            a.Ignore(addr => addr.IsUSAddress);
        });

        builder.Property(e => e.ReviewedBy)
            .HasConversion(
                v => v != null ? v.Value : (Guid?)null,
                v => v.HasValue ? new UserId(v.Value) : null);

        // Enum Conversions
        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Step)
            .HasConversion<int>()
            .IsRequired();

        // JSON Properties
        builder.Property(e => e.PersonalDetails)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<PersonalDetails>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(5000);

        builder.Property(e => e.ContactInformation)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<ContactInformation>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(5000);

        builder.Property(e => e.ForeignIdentification)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<ForeignIdentification>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(1000);

        // List Properties (stored as JSON)
        builder.Property(e => e.Dependents)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<List<Dependent>>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(10000);

        builder.Property(e => e.ResidencyHistory)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<List<Residency>>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(10000);

        builder.Property(e => e.ImmigrationHistories)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<List<ImmigrationHistory>>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(10000);

        builder.Property(e => e.EducationQualifications)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<List<EducationQualification>>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(10000);

        builder.Property(e => e.EmploymentHistory)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<List<EmploymentHistory>>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(10000);

        builder.Property(e => e.ForeignCitizenshipApplications)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<List<ForeignCitizenshipApplication>>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(10000);

        builder.Property(e => e.CriminalHistory)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<List<CriminalHistory>>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(10000);

        builder.Property(e => e.References)
            .HasConversion(
                v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
                v => !string.IsNullOrEmpty(v) ? JsonSerializer.Deserialize<List<Reference>>(v, (JsonSerializerOptions?)null) : null)
            .HasMaxLength(10000);

        // String Properties
        builder.Property(e => e.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(e => e.ExtendedDataJson)
            .HasMaxLength(50000);

        builder.Property(e => e.AccessLogsJson)
            .HasMaxLength(50000);

        builder.Property(e => e.ApprovedBy)
            .HasMaxLength(255);

        builder.Property(e => e.RejectedBy)
            .HasMaxLength(255);

        // Date Properties
        builder.Property(e => e.DateOfBirth)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // Decimal Properties
        builder.Property(e => e.Fee)
            .HasPrecision(18, 2);

        builder.Property(e => e.IdentificationCardFee)
            .HasPrecision(18, 2);

        // Concurrency Token
        builder.Property(e => e.Version)
            .IsConcurrencyToken();

        // Configure Uploads navigation relationship
        builder.HasMany(e => e.Uploads)
            .WithOne(d => d.Application)
            .HasForeignKey(d => d.ApplicationId)
            .HasPrincipalKey(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore Computed Properties
        builder.Ignore(e => e.FirstName);
        builder.Ignore(e => e.LastName);
        builder.Ignore(e => e.IsPaymentProcessed);
        builder.Ignore(e => e.Events);

        // Indexes
        // Unique index on ApplicationNumber
        builder.HasIndex(e => e.ApplicationNumber)
            .IsUnique()
            .HasDatabaseName("IX_applications_application_number");

        // Single column indexes
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_applications_tenant_id");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_applications_status");

        builder.HasIndex(e => e.Step)
            .HasDatabaseName("IX_applications_step");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_applications_created_at");

        builder.HasIndex(e => e.UpdatedAt)
            .HasDatabaseName("IX_applications_updated_at");

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("IX_applications_email")
            .HasFilter("\"email\" IS NOT NULL");

        builder.HasIndex(e => e.PaymentTransactionId)
            .HasDatabaseName("IX_applications_payment_transaction_id")
            .HasFilter("\"payment_transaction_id\" IS NOT NULL");

        builder.HasIndex(e => e.SubmittedAt)
            .HasDatabaseName("IX_applications_submitted_at")
            .HasFilter("\"submitted_at\" IS NOT NULL");

        builder.HasIndex(e => e.ApprovedAt)
            .HasDatabaseName("IX_applications_approved_at")
            .HasFilter("\"approved_at\" IS NOT NULL");

        builder.HasIndex(e => e.RejectedAt)
            .HasDatabaseName("IX_applications_rejected_at")
            .HasFilter("\"rejected_at\" IS NOT NULL");

        // Composite indexes for common query patterns
        builder.HasIndex(e => new { e.TenantId, e.Status })
            .HasDatabaseName("IX_applications_tenant_id_status");

        builder.HasIndex(e => new { e.TenantId, e.Step })
            .HasDatabaseName("IX_applications_tenant_id_step");

        builder.HasIndex(e => new { e.TenantId, e.Status, e.Step })
            .HasDatabaseName("IX_applications_tenant_id_status_step");

        builder.HasIndex(e => new { e.TenantId, e.CreatedAt })
            .HasDatabaseName("IX_applications_tenant_id_created_at");

        builder.HasIndex(e => new { e.Status, e.CreatedAt })
            .HasDatabaseName("IX_applications_status_created_at");

        builder.HasIndex(e => new { e.TenantId, e.Status, e.CreatedAt })
            .HasDatabaseName("IX_applications_tenant_id_status_created_at");

        // Index for payment processing queries
        builder.HasIndex(e => new { e.TenantId, e.PaymentTransactionId })
            .HasDatabaseName("IX_applications_tenant_id_payment_transaction_id")
            .HasFilter("\"payment_transaction_id\" IS NOT NULL");

        // Index for review tracking
        builder.HasIndex(e => new { e.TenantId, e.ReviewedBy })
            .HasDatabaseName("IX_applications_tenant_id_reviewed_by")
            .HasFilter("\"reviewed_by\" IS NOT NULL");
    }
}

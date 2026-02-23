using System;
using Pupitre.Curricula.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.Curricula.Infrastructure.EF.Configuration;

public class CurriculumConfiguration : IEntityTypeConfiguration<Curriculum>
{
    public CurriculumConfiguration()
    {
    }

    void IEntityTypeConfiguration<Curriculum>.Configure(EntityTypeBuilder<Curriculum> builder)
    {
        builder.ToTable("curriculum");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new CurriculumId(c))
        .IsRequired();

        // Tags Collection (assuming many-to-many or storing as a serialized array)
        builder.Property(c => c.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .IsRequired(false);

        builder.Ignore(c => c.Events);
        builder.Property(c => c.CitizenId).HasColumnName("citizen_id");
        builder.Property(c => c.Nationality).HasColumnName("nationality");
        builder.Property(c => c.ProgramCode).HasColumnName("program_code");
        builder.Property(c => c.CredentialType).HasColumnName("credential_type");
        builder.Property(c => c.CompletionDate).HasColumnName("completion_date");
        builder.Property(c => c.GovernmentIdentityId).HasColumnName("government_identity_id");
        builder.Property(c => c.BlockchainAccount).HasColumnName("blockchain_account");
        builder.Property(c => c.CredentialDocumentId).HasColumnName("credential_document_id");
        builder.Property(c => c.CredentialDocumentHash).HasColumnName("credential_document_hash");
        builder.Property(c => c.LedgerTransactionId).HasColumnName("ledger_transaction_id");
        builder.Property(c => c.CredentialIssuedAt).HasColumnName("credential_issued_at");
        builder.Property(c => c.CredentialStatus).HasColumnName("credential_status");
        builder.Property(c => c.BlockchainMetadata).HasColumnName("blockchain_metadata");
        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique(false);
    }
}


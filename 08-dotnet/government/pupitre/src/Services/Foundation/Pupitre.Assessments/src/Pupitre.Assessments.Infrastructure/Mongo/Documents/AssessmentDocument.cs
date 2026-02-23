using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Assessments.Application.DTO;
using Pupitre.Assessments.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Assessments.Tests.Integration.Async")]
namespace Pupitre.Assessments.Infrastructure.Mongo.Documents;

internal class AssessmentDocument : IIdentifiable<Guid>
{
    public AssessmentDocument()
    {

    }

    public AssessmentDocument(Assessment assessment)
    {
        if (assessment is null)
        {
            throw new NullReferenceException();
        }

        Id = assessment.Id.Value;
        Name = assessment.Name;
        CreatedAt = assessment.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = assessment.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = assessment.Tags;
        Version = assessment.Version;
        CitizenId = assessment.CitizenId;
        Nationality = assessment.Nationality;
        ProgramCode = assessment.ProgramCode;
        CredentialType = assessment.CredentialType;
        CompletionDate = assessment.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = assessment.GovernmentIdentityId;
        BlockchainAccount = assessment.BlockchainAccount;
        CredentialDocumentId = assessment.CredentialDocumentId;
        CredentialDocumentHash = assessment.CredentialDocumentHash;
        LedgerTransactionId = assessment.LedgerTransactionId;
        CredentialIssuedAt = assessment.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = assessment.CredentialStatus;
        BlockchainMetadata = assessment.GetBlockchainMetadata();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }
    public string? CitizenId { get; set; }
    public string? Nationality { get; set; }
    public string? ProgramCode { get; set; }
    public string? CredentialType { get; set; }
    public long? CompletionDate { get; set; }
    public string? GovernmentIdentityId { get; set; }
    public string? BlockchainAccount { get; set; }
    public string? CredentialDocumentId { get; set; }
    public string? CredentialDocumentHash { get; set; }
    public string? LedgerTransactionId { get; set; }
    public long? CredentialIssuedAt { get; set; }
    public string? CredentialStatus { get; set; }
    public IReadOnlyDictionary<string, string>? BlockchainMetadata { get; set; }

    public Assessment AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public AssessmentDto AsDto()
        => new AssessmentDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public AssessmentDetailsDto AsDetailsDto()
        => new AssessmentDetailsDto(
            this.AsDto(),
            CreatedAt.GetDate(),
            ModifiedAt?.GetDate(),
            CompletionDate?.GetDate(),
            CredentialIssuedAt?.GetDate(),
            CredentialDocumentId,
            LedgerTransactionId);

    private static string? SerializeMetadata(IReadOnlyDictionary<string, string>? metadata)
        => metadata is { Count: > 0 } ? JsonSerializer.Serialize(metadata) : null;
}


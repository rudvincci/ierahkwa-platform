using System.Collections.Generic;
using Pupitre.Curricula.Application.DTO;
using Pupitre.Curricula.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("Pupitre.Curricula.Tests.Integration.Async")]
namespace Pupitre.Curricula.Infrastructure.Mongo.Documents;

internal class CurriculumDocument : IIdentifiable<Guid>
{
    public CurriculumDocument()
    {

    }

    public CurriculumDocument(Curriculum curriculum)
    {
        if (curriculum is null)
        {
            throw new NullReferenceException();
        }

        Id = curriculum.Id.Value;
        Name = curriculum.Name;
        CreatedAt = curriculum.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = curriculum.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = curriculum.Tags;
        Version = curriculum.Version;
        CitizenId = curriculum.CitizenId;
        Nationality = curriculum.Nationality;
        ProgramCode = curriculum.ProgramCode;
        CredentialType = curriculum.CredentialType;
        CompletionDate = curriculum.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = curriculum.GovernmentIdentityId;
        BlockchainAccount = curriculum.BlockchainAccount;
        CredentialDocumentId = curriculum.CredentialDocumentId;
        CredentialDocumentHash = curriculum.CredentialDocumentHash;
        LedgerTransactionId = curriculum.LedgerTransactionId;
        CredentialIssuedAt = curriculum.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = curriculum.CredentialStatus;
        BlockchainMetadata = curriculum.GetBlockchainMetadata();
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

    public Curriculum AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public CurriculumDto AsDto()
        => new CurriculumDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public CurriculumDetailsDto AsDetailsDto()
        => new CurriculumDetailsDto(
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


using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Progress.Application.DTO;
using Pupitre.Progress.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Progress.Tests.Integration.Async")]
namespace Pupitre.Progress.Infrastructure.Mongo.Documents;

internal class LearningProgressDocument : IIdentifiable<Guid>
{
    public LearningProgressDocument()
    {

    }

    public LearningProgressDocument(LearningProgress learningprogress)
    {
        if (learningprogress is null)
        {
            throw new NullReferenceException();
        }

        Id = learningprogress.Id.Value;
        Name = learningprogress.Name;
        CreatedAt = learningprogress.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = learningprogress.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = learningprogress.Tags;
        Version = learningprogress.Version;
        CitizenId = learningprogress.CitizenId;
        Nationality = learningprogress.Nationality;
        ProgramCode = learningprogress.ProgramCode;
        CredentialType = learningprogress.CredentialType;
        CompletionDate = learningprogress.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = learningprogress.GovernmentIdentityId;
        BlockchainAccount = learningprogress.BlockchainAccount;
        CredentialDocumentId = learningprogress.CredentialDocumentId;
        CredentialDocumentHash = learningprogress.CredentialDocumentHash;
        LedgerTransactionId = learningprogress.LedgerTransactionId;
        CredentialIssuedAt = learningprogress.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = learningprogress.CredentialStatus;
        BlockchainMetadata = learningprogress.GetBlockchainMetadata();
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

    public LearningProgress AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public LearningProgressDto AsDto()
        => new LearningProgressDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public LearningProgressDetailsDto AsDetailsDto()
        => new LearningProgressDetailsDto(
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


using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Parents.Application.DTO;
using Pupitre.Parents.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Parents.Tests.Integration.Async")]
namespace Pupitre.Parents.Infrastructure.Mongo.Documents;

internal class ParentDocument : IIdentifiable<Guid>
{
    public ParentDocument()
    {

    }

    public ParentDocument(Parent parent)
    {
        if (parent is null)
        {
            throw new NullReferenceException();
        }

        Id = parent.Id.Value;
        Name = parent.Name;
        CreatedAt = parent.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = parent.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = parent.Tags;
        Version = parent.Version;
        CitizenId = parent.CitizenId;
        Nationality = parent.Nationality;
        ProgramCode = parent.ProgramCode;
        CredentialType = parent.CredentialType;
        CompletionDate = parent.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = parent.GovernmentIdentityId;
        BlockchainAccount = parent.BlockchainAccount;
        CredentialDocumentId = parent.CredentialDocumentId;
        CredentialDocumentHash = parent.CredentialDocumentHash;
        LedgerTransactionId = parent.LedgerTransactionId;
        CredentialIssuedAt = parent.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = parent.CredentialStatus;
        BlockchainMetadata = parent.GetBlockchainMetadata();
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

    public Parent AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public ParentDto AsDto()
        => new ParentDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public ParentDetailsDto AsDetailsDto()
        => new ParentDetailsDto(
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


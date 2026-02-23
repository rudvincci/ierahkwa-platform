using System.Collections.Generic;
using System.Text.Json;
using Pupitre.GLEs.Application.DTO;
using Pupitre.GLEs.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.GLEs.Tests.Integration.Async")]
namespace Pupitre.GLEs.Infrastructure.Mongo.Documents;

internal class GLEDocument : IIdentifiable<Guid>
{
    public GLEDocument()
    {

    }

    public GLEDocument(GLE gle)
    {
        if (gle is null)
        {
            throw new NullReferenceException();
        }

        Id = gle.Id.Value;
        Name = gle.Name;
        CreatedAt = gle.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = gle.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = gle.Tags;
        Version = gle.Version;
        CitizenId = gle.CitizenId;
        Nationality = gle.Nationality;
        ProgramCode = gle.ProgramCode;
        CredentialType = gle.CredentialType;
        CompletionDate = gle.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = gle.GovernmentIdentityId;
        BlockchainAccount = gle.BlockchainAccount;
        CredentialDocumentId = gle.CredentialDocumentId;
        CredentialDocumentHash = gle.CredentialDocumentHash;
        LedgerTransactionId = gle.LedgerTransactionId;
        CredentialIssuedAt = gle.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = gle.CredentialStatus;
        BlockchainMetadata = gle.GetBlockchainMetadata();
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

    public GLE AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public GLEDto AsDto()
        => new GLEDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public GLEDetailsDto AsDetailsDto()
        => new GLEDetailsDto(
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


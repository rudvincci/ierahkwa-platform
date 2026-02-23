using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Educators.Application.DTO;
using Pupitre.Educators.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Educators.Tests.Integration.Async")]
namespace Pupitre.Educators.Infrastructure.Mongo.Documents;

internal class EducatorDocument : IIdentifiable<Guid>
{
    public EducatorDocument()
    {

    }

    public EducatorDocument(Educator educator)
    {
        if (educator is null)
        {
            throw new NullReferenceException();
        }

        Id = educator.Id.Value;
        Name = educator.Name;
        CreatedAt = educator.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = educator.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = educator.Tags;
        Version = educator.Version;
        CitizenId = educator.CitizenId;
        Nationality = educator.Nationality;
        ProgramCode = educator.ProgramCode;
        CredentialType = educator.CredentialType;
        CompletionDate = educator.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = educator.GovernmentIdentityId;
        BlockchainAccount = educator.BlockchainAccount;
        CredentialDocumentId = educator.CredentialDocumentId;
        CredentialDocumentHash = educator.CredentialDocumentHash;
        LedgerTransactionId = educator.LedgerTransactionId;
        CredentialIssuedAt = educator.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = educator.CredentialStatus;
        BlockchainMetadata = educator.GetBlockchainMetadata();
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

    public Educator AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public EducatorDto AsDto()
        => new EducatorDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public EducatorDetailsDto AsDetailsDto()
        => new EducatorDetailsDto(
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


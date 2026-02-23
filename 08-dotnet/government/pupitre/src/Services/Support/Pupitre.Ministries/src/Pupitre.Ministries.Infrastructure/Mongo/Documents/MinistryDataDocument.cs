using System.Collections.Generic;
using Pupitre.Ministries.Application.DTO;
using Pupitre.Ministries.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("Pupitre.Ministries.Tests.Integration.Async")]
namespace Pupitre.Ministries.Infrastructure.Mongo.Documents;

internal class MinistryDataDocument : IIdentifiable<Guid>
{
    public MinistryDataDocument()
    {

    }

    public MinistryDataDocument(MinistryData ministrydata)
    {
        if (ministrydata is null)
        {
            throw new NullReferenceException();
        }

        Id = ministrydata.Id.Value;
        Name = ministrydata.Name;
        CreatedAt = ministrydata.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = ministrydata.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = ministrydata.Tags;
        Version = ministrydata.Version;
        CitizenId = ministrydata.CitizenId;
        ProgramCode = ministrydata.ProgramCode;
        CredentialType = ministrydata.CredentialType;
        CompletionDate = ministrydata.CompletionDate?.ToUnixTimeMilliseconds();
        Nationality = ministrydata.Nationality;
        GovernmentIdentityId = ministrydata.GovernmentIdentityId;
        BlockchainAccount = ministrydata.BlockchainAccount;
        CredentialDocumentId = ministrydata.CredentialDocumentId;
        CredentialDocumentHash = ministrydata.CredentialDocumentHash;
        LedgerTransactionId = ministrydata.LedgerTransactionId;
        CredentialIssuedAt = ministrydata.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = ministrydata.CredentialStatus;
        BlockchainMetadata = ministrydata.GetBlockchainMetadata();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }
    public string? CitizenId { get; set; }
    public string? ProgramCode { get; set; }
    public string? CredentialType { get; set; }
    public long? CompletionDate { get; set; }
    public string? Nationality { get; set; }
    public string? GovernmentIdentityId { get; set; }
    public string? BlockchainAccount { get; set; }
    public string? CredentialDocumentId { get; set; }
    public string? CredentialDocumentHash { get; set; }
    public string? LedgerTransactionId { get; set; }
    public long? CredentialIssuedAt { get; set; }
    public string? CredentialStatus { get; set; }
    public IReadOnlyDictionary<string, string>? BlockchainMetadata { get; set; }

    public MinistryData AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Nationality, Version);

    public MinistryDataDto AsDto()
        => new MinistryDataDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);

    public MinistryDataDetailsDto AsDetailsDto()
        => new MinistryDataDetailsDto(
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


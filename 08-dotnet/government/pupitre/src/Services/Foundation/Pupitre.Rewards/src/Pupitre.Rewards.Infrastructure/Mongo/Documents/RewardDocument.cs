using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Rewards.Application.DTO;
using Pupitre.Rewards.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Rewards.Tests.Integration.Async")]
namespace Pupitre.Rewards.Infrastructure.Mongo.Documents;

internal class RewardDocument : IIdentifiable<Guid>
{
    public RewardDocument()
    {

    }

    public RewardDocument(Reward reward)
    {
        if (reward is null)
        {
            throw new NullReferenceException();
        }

        Id = reward.Id.Value;
        Name = reward.Name;
        CreatedAt = reward.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = reward.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = reward.Tags;
        Version = reward.Version;
        CitizenId = reward.CitizenId;
        Nationality = reward.Nationality;
        ProgramCode = reward.ProgramCode;
        CredentialType = reward.CredentialType;
        CompletionDate = reward.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = reward.GovernmentIdentityId;
        BlockchainAccount = reward.BlockchainAccount;
        CredentialDocumentId = reward.CredentialDocumentId;
        CredentialDocumentHash = reward.CredentialDocumentHash;
        LedgerTransactionId = reward.LedgerTransactionId;
        CredentialIssuedAt = reward.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = reward.CredentialStatus;
        BlockchainMetadata = reward.GetBlockchainMetadata();
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

    public Reward AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public RewardDto AsDto()
        => new RewardDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public RewardDetailsDto AsDetailsDto()
        => new RewardDetailsDto(
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


using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Users.Application.DTO;
using Pupitre.Users.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Users.Tests.Integration.Async")]
namespace Pupitre.Users.Infrastructure.Mongo.Documents;

internal class UserDocument : IIdentifiable<Guid>
{
    public UserDocument()
    {

    }

    public UserDocument(User user)
    {
        if (user is null)
        {
            throw new NullReferenceException();
        }

        Id = user.Id.Value;
        Name = user.Name;
        CreatedAt = user.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = user.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = user.Tags;
        Version = user.Version;
        CitizenId = user.CitizenId;
        Nationality = user.Nationality;
        ProgramCode = user.ProgramCode;
        CredentialType = user.CredentialType;
        CompletionDate = user.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = user.GovernmentIdentityId;
        BlockchainAccount = user.BlockchainAccount;
        CredentialDocumentId = user.CredentialDocumentId;
        CredentialDocumentHash = user.CredentialDocumentHash;
        LedgerTransactionId = user.LedgerTransactionId;
        CredentialIssuedAt = user.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = user.CredentialStatus;
        BlockchainMetadata = user.GetBlockchainMetadata();
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

    public User AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public UserDto AsDto()
        => new UserDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public UserDetailsDto AsDetailsDto()
        => new UserDetailsDto(
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


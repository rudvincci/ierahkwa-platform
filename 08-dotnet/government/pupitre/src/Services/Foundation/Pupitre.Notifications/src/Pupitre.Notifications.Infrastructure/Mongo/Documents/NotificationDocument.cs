using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Notifications.Application.DTO;
using Pupitre.Notifications.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Notifications.Tests.Integration.Async")]
namespace Pupitre.Notifications.Infrastructure.Mongo.Documents;

internal class NotificationDocument : IIdentifiable<Guid>
{
    public NotificationDocument()
    {

    }

    public NotificationDocument(Notification notification)
    {
        if (notification is null)
        {
            throw new NullReferenceException();
        }

        Id = notification.Id.Value;
        Name = notification.Name;
        CreatedAt = notification.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = notification.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = notification.Tags;
        Version = notification.Version;
        CitizenId = notification.CitizenId;
        Nationality = notification.Nationality;
        ProgramCode = notification.ProgramCode;
        CredentialType = notification.CredentialType;
        CompletionDate = notification.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = notification.GovernmentIdentityId;
        BlockchainAccount = notification.BlockchainAccount;
        CredentialDocumentId = notification.CredentialDocumentId;
        CredentialDocumentHash = notification.CredentialDocumentHash;
        LedgerTransactionId = notification.LedgerTransactionId;
        CredentialIssuedAt = notification.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = notification.CredentialStatus;
        BlockchainMetadata = notification.GetBlockchainMetadata();
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

    public Notification AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public NotificationDto AsDto()
        => new NotificationDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public NotificationDetailsDto AsDetailsDto()
        => new NotificationDetailsDto(
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


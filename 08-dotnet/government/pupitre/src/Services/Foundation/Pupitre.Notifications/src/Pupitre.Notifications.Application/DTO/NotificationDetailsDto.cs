using System.Collections.Generic;
using Mamey.Types;

namespace Pupitre.Notifications.Application.DTO;

internal class NotificationDetailsDto : NotificationDto
{
    public NotificationDetailsDto(
        Guid id,
        string name,
        IEnumerable<string> tags,
        DateTime createdAt,
        DateTime? modifiedAt,
        DateTime? completionDate = null,
        DateTime? credentialIssuedAt = null,
        string? credentialDocumentId = null,
        string? ledgerTransactionId = null,
        string? programCode = null,
        string? credentialType = null,
        string? citizenId = null,
        string? governmentIdentityId = null,
        string? blockchainAccount = null,
        string? credentialStatus = null,
        string? nationality = null,
        IReadOnlyDictionary<string, string>? blockchainMetadata = null)
        : base(id, name, tags, programCode, credentialType, citizenId, governmentIdentityId, blockchainAccount, credentialStatus, nationality, blockchainMetadata)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        CompletionDate = completionDate;
        CredentialIssuedAt = credentialIssuedAt;
        CredentialDocumentId = credentialDocumentId;
        LedgerTransactionId = ledgerTransactionId;
    }
    public NotificationDetailsDto(
        NotificationDto notificationDto,
        DateTime createdAt,
        DateTime? modifiedAt,
        DateTime? completionDate = null,
        DateTime? credentialIssuedAt = null,
        string? credentialDocumentId = null,
        string? ledgerTransactionId = null)
        : base(notificationDto.Id, notificationDto.Name, notificationDto.Tags,
            notificationDto.ProgramCode, notificationDto.CredentialType, notificationDto.CitizenId,
            notificationDto.GovernmentIdentityId, notificationDto.BlockchainAccount, notificationDto.CredentialStatus,
            notificationDto.Nationality, notificationDto.BlockchainMetadata)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        CompletionDate = completionDate;
        CredentialIssuedAt = credentialIssuedAt;
        CredentialDocumentId = credentialDocumentId;
        LedgerTransactionId = ledgerTransactionId;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime? CredentialIssuedAt { get; set; }
    public string? CredentialDocumentId { get; set; }
    public string? LedgerTransactionId { get; set; }
}

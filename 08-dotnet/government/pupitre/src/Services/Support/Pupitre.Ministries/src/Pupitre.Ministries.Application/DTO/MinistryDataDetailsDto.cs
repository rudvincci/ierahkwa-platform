using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Ministries.Application.DTO;

internal class MinistryDataDetailsDto : MinistryDataDto
{
    public MinistryDataDetailsDto(
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

    public MinistryDataDetailsDto(
        MinistryDataDto ministrydataDto,
        DateTime createdAt,
        DateTime? modifiedAt,
        DateTime? completionDate = null,
        DateTime? credentialIssuedAt = null,
        string? credentialDocumentId = null,
        string? ledgerTransactionId = null)
        : base(ministrydataDto.Id, ministrydataDto.Name, ministrydataDto.Tags, ministrydataDto.ProgramCode,
            ministrydataDto.CredentialType, ministrydataDto.CitizenId, ministrydataDto.GovernmentIdentityId,
            ministrydataDto.BlockchainAccount, ministrydataDto.CredentialStatus, ministrydataDto.Nationality, ministrydataDto.BlockchainMetadata)
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

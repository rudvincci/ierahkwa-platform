using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Curricula.Application.DTO;

internal class CurriculumDetailsDto : CurriculumDto
{
    public CurriculumDetailsDto(
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
    public CurriculumDetailsDto(
        CurriculumDto curriculumDto,
        DateTime createdAt,
        DateTime? modifiedAt,
        DateTime? completionDate = null,
        DateTime? credentialIssuedAt = null,
        string? credentialDocumentId = null,
        string? ledgerTransactionId = null)
        : base(curriculumDto.Id, curriculumDto.Name, curriculumDto.Tags, curriculumDto.ProgramCode,
            curriculumDto.CredentialType, curriculumDto.CitizenId, curriculumDto.GovernmentIdentityId,
            curriculumDto.BlockchainAccount, curriculumDto.CredentialStatus, curriculumDto.Nationality, curriculumDto.BlockchainMetadata)
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

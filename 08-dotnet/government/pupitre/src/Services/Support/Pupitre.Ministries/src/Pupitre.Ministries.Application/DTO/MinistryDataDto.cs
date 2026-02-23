using System.Collections.Generic;

namespace Pupitre.Ministries.Application.DTO;

public class MinistryDataDto
{
    public MinistryDataDto(
        Guid id,
        string name,
        IEnumerable<string> tags,
        string? programCode = null,
        string? credentialType = null,
        string? citizenId = null,
        string? governmentIdentityId = null,
        string? blockchainAccount = null,
        string? credentialStatus = null,
        string? nationality = null,
        IReadOnlyDictionary<string, string>? blockchainMetadata = null)
    {
        Id = id;
        Name = name;
        Tags = tags;
        ProgramCode = programCode;
        CredentialType = credentialType;
        CitizenId = citizenId;
        Nationality = nationality;
        GovernmentIdentityId = governmentIdentityId;
        BlockchainAccount = blockchainAccount;
        CredentialStatus = credentialStatus;
        BlockchainMetadata = blockchainMetadata;
    }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public string? ProgramCode { get; set; }
    public string? CredentialType { get; set; }
    public string? CitizenId { get; set; }
    public string? Nationality { get; set; }
    public string? GovernmentIdentityId { get; set; }
    public string? BlockchainAccount { get; set; }
    public string? CredentialStatus { get; set; }
    public IReadOnlyDictionary<string, string>? BlockchainMetadata { get; set; }
}

using Pupitre.Types;

namespace Pupitre.Blockchain.Models;

/// <summary>
/// Payload describing the educational credential that must be notarized on MameyNode.
/// </summary>
public sealed class EducationLedgerPayload
{
    public Guid MinistryDataId { get; init; }
    public string? CitizenId { get; init; }
    public string? IdentityId { get; init; }
    public string? BlockchainAccount { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? Nationality { get; init; }
    public string? ProgramCode { get; init; }
    public string? CredentialType { get; init; }
    public DateTime? CompletionDate { get; init; }
    public string? CredentialDocumentBase64 { get; init; }
    public string CredentialMimeType { get; init; } = "application/pdf";
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
    public string? TransactionId { get; init; }
    public string? SourceAccount { get; init; }
    public string? TargetAccount { get; init; }
    public string? Amount { get; init; }
    public string? Currency { get; init; }
}

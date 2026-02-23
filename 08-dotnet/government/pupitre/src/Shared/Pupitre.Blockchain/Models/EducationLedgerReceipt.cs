namespace Pupitre.Blockchain.Models;

/// <summary>
/// Result returned once the credential is notarized on-chain and via government services.
/// </summary>
public sealed class EducationLedgerReceipt
{
    public string? IdentityId { get; init; }
    public string? BlockchainAccount { get; init; }
    public string? DocumentId { get; init; }
    public string? DocumentHash { get; init; }
    public string? LedgerTransactionId { get; init; }
    public DateTime CredentialIssuedAt { get; init; } = DateTime.UtcNow;
    public bool PublishedToLedger { get; init; }
    public string? Notes { get; init; }
}

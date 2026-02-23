using System;

namespace Mamey.FWID.Sagas.Core.Data;

/// <summary>
/// Saga data structure for tracking Credential Issuance process.
/// Orchestrates: Identity Validation → Credential Issuance → ZKP Proof Generation → Ledger Logging
/// </summary>
internal class CredentialIssuanceSagaData
{
    public Guid IdentityId { get; set; }
    public string CredentialType { get; set; } = string.Empty;
    public Dictionary<string, object> Claims { get; set; } = new();
    public Guid IssuerId { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IdentityValidated { get; set; }
    public bool CredentialIssued { get; set; }
    public bool ZKPProofGenerated { get; set; }
    public bool LedgerLogged { get; set; }
    public string Status { get; set; } = "Pending";
    public Guid? CredentialId { get; set; }
    public Guid? ProofId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}




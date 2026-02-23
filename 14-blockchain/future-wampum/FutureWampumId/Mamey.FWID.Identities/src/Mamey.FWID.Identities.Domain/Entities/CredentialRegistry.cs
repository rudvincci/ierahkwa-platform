using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a credential registry aggregate root for public verification.
/// </summary>
internal class CredentialRegistry : AggregateRoot<CredentialRegistryId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private CredentialRegistry()
    {
        RevocationList = new List<RevocationEntry>();
        VerificationHistory = new List<VerificationRecord>();
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the CredentialRegistry aggregate root.
    /// </summary>
    /// <param name="id">The credential registry identifier.</param>
    /// <param name="credentialId">The credential identifier.</param>
    /// <param name="issuerDid">The DID of the credential issuer.</param>
    /// <param name="credentialHash">The hash of the credential for verification.</param>
    /// <param name="credentialType">The type of credential.</param>
    public CredentialRegistry(
        CredentialRegistryId id,
        string credentialId,
        string issuerDid,
        string credentialHash,
        string credentialType)
        : base(id)
    {
        CredentialId = credentialId ?? throw new ArgumentNullException(nameof(credentialId));
        IssuerDid = issuerDid ?? throw new ArgumentNullException(nameof(issuerDid));
        CredentialHash = credentialHash ?? throw new ArgumentNullException(nameof(credentialHash));
        CredentialType = credentialType ?? throw new ArgumentNullException(nameof(credentialType));
        Status = RegistryStatus.Active;
        RegisteredAt = DateTime.UtcNow;
        LastVerifiedAt = DateTime.UtcNow;
        RevocationList = new List<RevocationEntry>();
        VerificationHistory = new List<VerificationRecord>();
        Metadata = new Dictionary<string, object>();
        Version = 1;

        AddEvent(new CredentialRegistered(Id, CredentialId, IssuerDid, CredentialType, RegisteredAt));
    }

    #region Properties

    /// <summary>
    /// The credential identifier.
    /// </summary>
    public string CredentialId { get; private set; }

    /// <summary>
    /// The DID of the credential issuer.
    /// </summary>
    public string IssuerDid { get; private set; }

    /// <summary>
    /// The hash of the credential for verification.
    /// </summary>
    public string CredentialHash { get; private set; }

    /// <summary>
    /// The type of credential.
    /// </summary>
    public string CredentialType { get; private set; }

    /// <summary>
    /// The current status of the credential in the registry.
    /// </summary>
    public RegistryStatus Status { get; private set; }

    /// <summary>
    /// When the credential was registered.
    /// </summary>
    public DateTime RegisteredAt { get; private set; }

    /// <summary>
    /// When the credential was last verified.
    /// </summary>
    public DateTime LastVerifiedAt { get; private set; }

    /// <summary>
    /// The expiration date of the credential.
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// The blockchain transaction hash for anchoring.
    /// </summary>
    public string? BlockchainTxHash { get; private set; }

    /// <summary>
    /// The revocation list for this credential.
    /// </summary>
    public List<RevocationEntry> RevocationList { get; private set; }

    /// <summary>
    /// The verification history.
    /// </summary>
    public List<VerificationRecord> VerificationHistory { get; private set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Anchors the credential to the blockchain.
    /// </summary>
    /// <param name="transactionHash">The blockchain transaction hash.</param>
    /// <param name="blockNumber">The block number.</param>
    /// <param name="anchoredAt">When the anchoring occurred.</param>
    public void AnchorToBlockchain(string transactionHash, long blockNumber, DateTime anchoredAt)
    {
        BlockchainTxHash = transactionHash;
        LastVerifiedAt = anchoredAt;
        IncrementVersion();

        AddEvent(new CredentialAnchored(Id, transactionHash, blockNumber, anchoredAt));
    }

    /// <summary>
    /// Records a verification attempt.
    /// </summary>
    /// <param name="verifierId">The identifier of the verifier.</param>
    /// <param name="verificationResult">The result of the verification.</param>
    /// <param name="confidenceScore">The confidence score.</param>
    /// <param name="verifiedAt">When the verification occurred.</param>
    public void RecordVerification(
        string verifierId,
        VerificationResult verificationResult,
        int confidenceScore,
        DateTime verifiedAt)
    {
        var record = new VerificationRecord(
            verifierId,
            verificationResult,
            confidenceScore,
            verifiedAt);

        VerificationHistory.Add(record);
        LastVerifiedAt = verifiedAt;
        IncrementVersion();

        AddEvent(new CredentialVerified(Id, verifierId, verificationResult, confidenceScore, verifiedAt));
    }

    /// <summary>
    /// Revokes the credential.
    /// </summary>
    /// <param name="reason">The reason for revocation.</param>
    /// <param name="revokedBy">The entity that revoked the credential.</param>
    /// <param name="revokedAt">When the revocation occurred.</param>
    public void Revoke(string reason, string revokedBy, DateTime revokedAt)
    {
        if (Status == RegistryStatus.Revoked)
            return;

        Status = RegistryStatus.Revoked;

        var revocationEntry = new RevocationEntry(
            reason,
            revokedBy,
            revokedAt);

        RevocationList.Add(revocationEntry);
        IncrementVersion();

        AddEvent(new CredentialRevoked(Id, reason, revokedBy, revokedAt));
    }

    /// <summary>
    /// Suspends the credential temporarily.
    /// </summary>
    /// <param name="reason">The reason for suspension.</param>
    /// <param name="suspendedBy">The entity that suspended the credential.</param>
    /// <param name="suspendedAt">When the suspension occurred.</param>
    public void Suspend(string reason, string suspendedBy, DateTime suspendedAt)
    {
        if (Status == RegistryStatus.Suspended)
            return;

        Status = RegistryStatus.Suspended;

        var revocationEntry = new RevocationEntry(
            reason,
            suspendedBy,
            suspendedAt);

        RevocationList.Add(revocationEntry);
        IncrementVersion();

        AddEvent(new CredentialSuspended(Id, reason, suspendedBy, suspendedAt));
    }

    /// <summary>
    /// Reactivates a suspended credential.
    /// </summary>
    /// <param name="reactivatedBy">The entity that reactivated the credential.</param>
    /// <param name="reactivatedAt">When the reactivation occurred.</param>
    public void Reactivate(string reactivatedBy, DateTime reactivatedAt)
    {
        if (Status != RegistryStatus.Suspended)
            throw new InvalidOperationException("Only suspended credentials can be reactivated");

        Status = RegistryStatus.Active;
        LastVerifiedAt = reactivatedAt;
        IncrementVersion();

        AddEvent(new CredentialReactivated(Id, reactivatedBy, reactivatedAt));
    }

    /// <summary>
    /// Updates the credential expiration.
    /// </summary>
    /// <param name="newExpiration">The new expiration date.</param>
    public void UpdateExpiration(DateTime? newExpiration)
    {
        ExpiresAt = newExpiration;
        IncrementVersion();

        AddEvent(new CredentialExpirationUpdated(Id, newExpiration ?? DateTime.UtcNow.AddYears(1), DateTime.UtcNow));
    }

    /// <summary>
    /// Verifies the credential hash against the registry.
    /// </summary>
    /// <param name="providedHash">The hash to verify.</param>
    /// <returns>The verification result.</returns>
    public CredentialVerificationResult VerifyCredential(string providedHash)
    {
        // Check if hashes match
        if (providedHash != CredentialHash)
        {
            return new CredentialVerificationResult(
                false,
                "Hash mismatch",
                Status,
                null,
                DateTime.UtcNow);
        }

        // Check if credential is active
        if (Status != RegistryStatus.Active)
        {
            return new CredentialVerificationResult(
                false,
                $"Credential is {Status.ToString().ToLower()}",
                Status,
                ExpiresAt,
                DateTime.UtcNow);
        }

        // Check if credential is expired
        if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow)
        {
            return new CredentialVerificationResult(
                false,
                "Credential has expired",
                RegistryStatus.Expired,
                ExpiresAt,
                DateTime.UtcNow);
        }

        return new CredentialVerificationResult(
            true,
            "Credential is valid",
            Status,
            ExpiresAt,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Checks if the credential is currently valid.
    /// </summary>
    /// <returns>True if the credential is active and not expired.</returns>
    public bool IsValid()
    {
        return Status == RegistryStatus.Active &&
               (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
    }

    /// <summary>
    /// Gets the latest revocation reason.
    /// </summary>
    /// <returns>The latest revocation entry, or null if not revoked.</returns>
    public RevocationEntry? GetLatestRevocation()
    {
        return RevocationList.OrderByDescending(r => r.RevokedAt).FirstOrDefault();
    }

    /// <summary>
    /// Gets the verification statistics.
    /// </summary>
    /// <returns>Statistics about verification attempts.</returns>
    public VerificationStatistics GetVerificationStatistics()
    {
        var totalVerifications = VerificationHistory.Count;
        var successfulVerifications = VerificationHistory.Count(v => v.Result == VerificationResult.Valid);
        var averageConfidence = VerificationHistory.Any() ?
            VerificationHistory.Average(v => v.ConfidenceScore) : 0;

        return new VerificationStatistics(
            totalVerifications,
            successfulVerifications,
            averageConfidence);
    }

    #endregion
}

/// <summary>
/// Represents the status of a credential in the registry.
/// </summary>
internal enum RegistryStatus
{
    Active,
    Suspended,
    Revoked,
    Expired
}

/// <summary>
/// Represents the result of a verification attempt.
/// </summary>
internal enum VerificationResult
{
    Valid,
    Invalid,
    Expired,
    Revoked,
    Suspended,
    Passed,
    Failed,
    RequiresManualReview,
    Incomplete
}

/// <summary>
/// Represents a revocation entry.
/// </summary>
internal class RevocationEntry
{
    public string Reason { get; set; }
    public string RevokedBy { get; set; }
    public DateTime RevokedAt { get; set; }

    public RevocationEntry(
        string reason,
        string revokedBy,
        DateTime revokedAt)
    {
        Reason = reason;
        RevokedBy = revokedBy;
        RevokedAt = revokedAt;
    }
}

/// <summary>
/// Represents a verification record.
/// </summary>
internal class VerificationRecord
{
    public string VerifierId { get; set; }
    public VerificationResult Result { get; set; }
    public int ConfidenceScore { get; set; }
    public DateTime VerifiedAt { get; set; }

    public VerificationRecord(
        string verifierId,
        VerificationResult result,
        int confidenceScore,
        DateTime verifiedAt)
    {
        VerifierId = verifierId;
        Result = result;
        ConfidenceScore = confidenceScore;
        VerifiedAt = verifiedAt;
    }
}

/// <summary>
/// Represents the result of a credential verification.
/// </summary>
internal class CredentialVerificationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public RegistryStatus Status { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime VerifiedAt { get; set; }

    public CredentialVerificationResult(
        bool isValid,
        string message,
        RegistryStatus status,
        DateTime? expiresAt,
        DateTime verifiedAt)
    {
        IsValid = isValid;
        Message = message;
        Status = status;
        ExpiresAt = expiresAt;
        VerifiedAt = verifiedAt;
    }
}

/// <summary>
/// Represents verification statistics.
/// </summary>
internal class VerificationStatistics
{
    public int TotalVerifications { get; set; }
    public int SuccessfulVerifications { get; set; }
    public double AverageConfidenceScore { get; set; }

    public VerificationStatistics(
        int totalVerifications,
        int successfulVerifications,
        double averageConfidenceScore)
    {
        TotalVerifications = totalVerifications;
        SuccessfulVerifications = successfulVerifications;
        AverageConfidenceScore = averageConfidenceScore;
    }
}

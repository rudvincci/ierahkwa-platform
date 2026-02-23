namespace Mamey.Blockchain.DID;

/// <summary>
/// Result of issuing a new DID.
/// </summary>
public record IssueDIDResult(
    bool Success,
    string? DID,
    string? DIDDocument,
    string? TransactionHash,
    string? ErrorMessage);

/// <summary>
/// Result of resolving a DID.
/// </summary>
public record ResolveDIDResult(
    bool Success,
    string? DIDDocument,
    DIDDocumentMetadata? Metadata,
    DIDResolutionMetadata? ResolutionMetadata,
    string? ErrorMessage);

/// <summary>
/// Result of updating a DID Document.
/// </summary>
public record UpdateDIDResult(
    bool Success,
    string? TransactionHash,
    ulong? NewVersion,
    string? ErrorMessage);

/// <summary>
/// Result of revoking a DID.
/// </summary>
public record RevokeDIDResult(
    bool Success,
    string? TransactionHash,
    string? ErrorMessage);

/// <summary>
/// Result of getting DID history.
/// </summary>
public record DIDHistoryResult(
    bool Success,
    IReadOnlyList<DIDHistoryEntry> Entries,
    int TotalCount,
    string? ErrorMessage);

/// <summary>
/// A single entry in the DID history.
/// </summary>
public record DIDHistoryEntry(
    ulong Version,
    string DIDDocument,
    string Action,
    string Actor,
    DateTime Timestamp,
    string TransactionHash,
    string? Reason);

/// <summary>
/// Result of verifying DID ownership.
/// </summary>
public record VerifyOwnershipResult(
    bool Success,
    bool Verified,
    string? VerificationMethodUsed,
    string? ErrorMessage);

/// <summary>
/// Result of listing DIDs.
/// </summary>
public record ListDIDsResult(
    bool Success,
    IReadOnlyList<DIDInfo> DIDs,
    int TotalCount,
    string? ErrorMessage);

/// <summary>
/// Information about a DID.
/// </summary>
public record DIDInfo(
    string DID,
    DIDStatus Status,
    DateTime Created,
    DateTime Updated,
    ulong Version);

/// <summary>
/// Result of modifying a DID (add/remove verification method or service).
/// </summary>
public record ModifyDIDResult(
    bool Success,
    string? TransactionHash,
    string? ErrorMessage);

/// <summary>
/// A verification method in a DID Document (W3C DID Core).
/// </summary>
public record DIDVerificationMethod(
    string Id,
    string Type,
    string Controller,
    string? PublicKeyMultibase = null,
    string? PublicKeyJwk = null);

/// <summary>
/// A service endpoint in a DID Document (W3C DID Core).
/// </summary>
public record DIDService(
    string Id,
    string Type,
    string ServiceEndpoint);

/// <summary>
/// DID Document metadata (W3C DID Resolution).
/// </summary>
public record DIDDocumentMetadata(
    DateTime? Created,
    DateTime? Updated,
    ulong VersionId,
    bool Deactivated,
    string? CanonicalId);

/// <summary>
/// DID Resolution metadata (W3C DID Resolution).
/// </summary>
public record DIDResolutionMetadata(
    string ContentType,
    string? Error,
    string? ErrorMessage,
    long Duration);

/// <summary>
/// Status of a DID.
/// </summary>
public enum DIDStatus
{
    Active = 0,
    Deactivated = 1,
    Suspended = 2
}

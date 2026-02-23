using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Enhanced credential service interface with multiple proof types and batch operations.
/// </summary>
public interface ICredentialService
{
    /// <summary>
    /// Issues a single verifiable credential.
    /// </summary>
    Task<VerifiableCredentialDto> IssueCredentialAsync(CredentialIssueRequest request);

    /// <summary>
    /// Issues multiple verifiable credentials in a batch operation.
    /// </summary>
    Task<BatchCredentialIssueResult> IssueCredentialsBatchAsync(BatchCredentialIssueRequest request);

    /// <summary>
    /// Issues a credential with specific proof type.
    /// </summary>
    Task<VerifiableCredentialDto> IssueCredentialWithProofTypeAsync(CredentialIssueRequest request, ProofType proofType);

    /// <summary>
    /// Verifies a verifiable credential.
    /// </summary>
    Task<CredentialVerificationResultDto> VerifyCredentialAsync(CredentialVerifyRequest request);

    /// <summary>
    /// Verifies multiple credentials in a batch operation.
    /// </summary>
    Task<BatchCredentialVerificationResult> VerifyCredentialsBatchAsync(BatchCredentialVerifyRequest request);

    /// <summary>
    /// Revokes a credential.
    /// </summary>
    Task RevokeCredentialAsync(string credentialId);

    /// <summary>
    /// Revokes multiple credentials in a batch operation.
    /// </summary>
    Task<BatchRevocationResult> RevokeCredentialsBatchAsync(BatchRevocationRequest request);

    /// <summary>
    /// Gets credential by ID.
    /// </summary>
    Task<VerifiableCredentialDto> GetCredentialAsync(string credentialId);

    /// <summary>
    /// Gets credentials by issuer DID.
    /// </summary>
    Task<IEnumerable<VerifiableCredentialDto>> GetCredentialsByIssuerAsync(string issuerDid);

    /// <summary>
    /// Gets credentials by subject DID.
    /// </summary>
    Task<IEnumerable<VerifiableCredentialDto>> GetCredentialsBySubjectAsync(string subjectDid);

    /// <summary>
    /// Updates credential status.
    /// </summary>
    Task UpdateCredentialStatusAsync(string credentialId, CredentialStatus status);

    /// <summary>
    /// Validates credential schema.
    /// </summary>
    Task<bool> ValidateSchemaAsync(string schemaRef, Dictionary<string, object> claims, CancellationToken cancellationToken = default);
}

/// <summary>
/// Proof types supported for credential issuance.
/// </summary>
public enum ProofType
{
    /// <summary>
    /// Ed25519 signature proof (default).
    /// </summary>
    Ed25519Signature2020,

    /// <summary>
    /// RSA signature proof.
    /// </summary>
    RsaSignature2018,

    /// <summary>
    /// ECDSA signature proof.
    /// </summary>
    EcdsaSecp256k1Signature2019,

    /// <summary>
    /// BBS+ signature proof for selective disclosure.
    /// </summary>
    BbsBlsSignature2020,

    /// <summary>
    /// JSON Web Signature proof.
    /// </summary>
    JsonWebSignature2020
}

/// <summary>
/// Batch credential issuance request.
/// </summary>
public class BatchCredentialIssueRequest
{
    /// <summary>
    /// List of credential issue requests.
    /// </summary>
    public List<CredentialIssueRequest> Requests { get; set; } = new();

    /// <summary>
    /// Default proof type for all credentials in the batch.
    /// </summary>
    public ProofType DefaultProofType { get; set; } = ProofType.Ed25519Signature2020;

    /// <summary>
    /// Whether to continue processing if some credentials fail.
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Optional batch metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Batch credential issuance result.
/// </summary>
public class BatchCredentialIssueResult
{
    /// <summary>
    /// Successfully issued credentials.
    /// </summary>
    public List<VerifiableCredentialDto> IssuedCredentials { get; set; } = new();

    /// <summary>
    /// Failed credential issuances.
    /// </summary>
    public List<CredentialIssueError> Errors { get; set; } = new();

    /// <summary>
    /// Total number of credentials processed.
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of successful issuances.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed issuances.
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Whether the batch operation was successful.
    /// </summary>
    public bool IsSuccess => ErrorCount == 0 || (ContinueOnError && SuccessCount > 0);

    /// <summary>
    /// Whether to continue on error (from request).
    /// </summary>
    public bool ContinueOnError { get; set; }
}

/// <summary>
/// Credential issue error details.
/// </summary>
public class CredentialIssueError
{
    /// <summary>
    /// The credential request that failed.
    /// </summary>
    public CredentialIssueRequest Request { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Error code.
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Exception details if available.
    /// </summary>
    public Exception Exception { get; set; }
}

/// <summary>
/// Batch credential verification request.
/// </summary>
public class BatchCredentialVerifyRequest
{
    /// <summary>
    /// List of credentials to verify.
    /// </summary>
    public List<VerifiableCredentialDto> Credentials { get; set; } = new();

    /// <summary>
    /// Whether to continue processing if some verifications fail.
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Optional verification options.
    /// </summary>
    public Dictionary<string, object> Options { get; set; } = new();
}

/// <summary>
/// Batch credential verification result.
/// </summary>
public class BatchCredentialVerificationResult
{
    /// <summary>
    /// Verification results for each credential.
    /// </summary>
    public List<CredentialVerificationResultDto> Results { get; set; } = new();

    /// <summary>
    /// Total number of credentials processed.
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of valid credentials.
    /// </summary>
    public int ValidCount { get; set; }

    /// <summary>
    /// Number of invalid credentials.
    /// </summary>
    public int InvalidCount { get; set; }

    /// <summary>
    /// Whether the batch verification was successful.
    /// </summary>
    public bool IsSuccess => InvalidCount == 0 || (ContinueOnError && ValidCount > 0);

    /// <summary>
    /// Whether to continue on error (from request).
    /// </summary>
    public bool ContinueOnError { get; set; }
}

/// <summary>
/// Batch revocation request.
/// </summary>
public class BatchRevocationRequest
{
    /// <summary>
    /// List of credential IDs to revoke.
    /// </summary>
    public List<string> CredentialIds { get; set; } = new();

    /// <summary>
    /// Revocation reason.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Whether to continue processing if some revocations fail.
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Optional revocation metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Batch revocation result.
/// </summary>
public class BatchRevocationResult
{
    /// <summary>
    /// Successfully revoked credential IDs.
    /// </summary>
    public List<string> RevokedCredentialIds { get; set; } = new();

    /// <summary>
    /// Failed revocation attempts.
    /// </summary>
    public List<RevocationError> Errors { get; set; } = new();

    /// <summary>
    /// Total number of credentials processed.
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of successful revocations.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed revocations.
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Whether the batch revocation was successful.
    /// </summary>
    public bool IsSuccess => ErrorCount == 0 || (ContinueOnError && SuccessCount > 0);

    /// <summary>
    /// Whether to continue on error (from request).
    /// </summary>
    public bool ContinueOnError { get; set; }
}

/// <summary>
/// Revocation error details.
/// </summary>
public class RevocationError
{
    /// <summary>
    /// The credential ID that failed to revoke.
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Error message.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Error code.
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Exception details if available.
    /// </summary>
    public Exception Exception { get; set; }
}
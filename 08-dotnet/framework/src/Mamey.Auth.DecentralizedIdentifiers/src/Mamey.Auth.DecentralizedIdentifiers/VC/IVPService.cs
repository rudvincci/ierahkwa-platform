using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Service for creating and validating Verifiable Presentations with selective disclosure capabilities.
/// </summary>
public interface IVPService
{
    /// <summary>
    /// Creates a Verifiable Presentation from one or more Verifiable Credentials.
    /// </summary>
    /// <param name="request">The presentation creation request</param>
    /// <returns>The created Verifiable Presentation</returns>
    Task<VerifiablePresentation> CreatePresentationAsync(PresentationCreateRequest request);

    /// <summary>
    /// Creates a Verifiable Presentation with selective disclosure.
    /// </summary>
    /// <param name="request">The selective disclosure request</param>
    /// <returns>The created Verifiable Presentation with selective disclosure</returns>
    Task<VerifiablePresentation> CreateSelectiveDisclosurePresentationAsync(SelectiveDisclosureRequest request);

    /// <summary>
    /// Validates a Verifiable Presentation.
    /// </summary>
    /// <param name="request">The validation request</param>
    /// <returns>The validation result</returns>
    Task<PresentationValidationResult> ValidatePresentationAsync(PresentationValidationRequest request);

    /// <summary>
    /// Validates multiple Verifiable Presentations in batch.
    /// </summary>
    /// <param name="request">The batch validation request</param>
    /// <returns>The batch validation result</returns>
    Task<BatchPresentationValidationResult> ValidatePresentationsBatchAsync(BatchPresentationValidationRequest request);

    /// <summary>
    /// Creates a presentation with zero-knowledge proof capabilities.
    /// </summary>
    /// <param name="request">The ZKP presentation request</param>
    /// <returns>The created ZKP Verifiable Presentation</returns>
    Task<VerifiablePresentation> CreateZkpPresentationAsync(ZkpPresentationRequest request);

    /// <summary>
    /// Extracts claims from a Verifiable Presentation.
    /// </summary>
    /// <param name="presentation">The Verifiable Presentation</param>
    /// <returns>Dictionary of extracted claims</returns>
    Task<Dictionary<string, object>> ExtractClaimsAsync(VerifiablePresentation presentation);

    /// <summary>
    /// Creates a presentation with challenge-response authentication.
    /// </summary>
    /// <param name="request">The challenge-response request</param>
    /// <returns>The created Verifiable Presentation</returns>
    Task<VerifiablePresentation> CreateChallengeResponsePresentationAsync(ChallengeResponseRequest request);

    /// <summary>
    /// Validates a presentation against a specific challenge.
    /// </summary>
    /// <param name="presentation">The Verifiable Presentation</param>
    /// <param name="expectedChallenge">The expected challenge</param>
    /// <returns>True if the challenge matches</returns>
    Task<bool> ValidateChallengeAsync(VerifiablePresentation presentation, string expectedChallenge);

    /// <summary>
    /// Creates a presentation with domain binding.
    /// </summary>
    /// <param name="request">The domain binding request</param>
    /// <returns>The created Verifiable Presentation</returns>
    Task<VerifiablePresentation> CreateDomainBoundPresentationAsync(DomainBindingRequest request);

    /// <summary>
    /// Validates domain binding in a presentation.
    /// </summary>
    /// <param name="presentation">The Verifiable Presentation</param>
    /// <param name="expectedDomain">The expected domain</param>
    /// <returns>True if the domain matches</returns>
    Task<bool> ValidateDomainBindingAsync(VerifiablePresentation presentation, string expectedDomain);
}

/// <summary>
/// Request for creating a Verifiable Presentation.
/// </summary>
public class PresentationCreateRequest
{
    /// <summary>
    /// The DID of the holder creating the presentation.
    /// </summary>
    public string HolderDid { get; set; } = string.Empty;

    /// <summary>
    /// The Verifiable Credentials to include in the presentation.
    /// </summary>
    public List<VerifiableCredential> Credentials { get; set; } = new();

    /// <summary>
    /// Optional challenge for anti-replay protection.
    /// </summary>
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// Optional domain for domain binding.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// The proof purpose (default: "authentication").
    /// </summary>
    public string ProofPurpose { get; set; } = "authentication";

    /// <summary>
    /// Optional presentation ID.
    /// </summary>
    public string PresentationId { get; set; } = string.Empty;

    /// <summary>
    /// Optional audience for the presentation.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Additional context URIs to include.
    /// </summary>
    public List<string> AdditionalContexts { get; set; } = new();

    /// <summary>
    /// Additional types to include.
    /// </summary>
    public List<string> AdditionalTypes { get; set; } = new();

    /// <summary>
    /// Whether to include credential status information.
    /// </summary>
    public bool IncludeCredentialStatus { get; set; } = true;

    /// <summary>
    /// Custom metadata to include in the presentation.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Request for creating a Verifiable Presentation with selective disclosure.
/// </summary>
public class SelectiveDisclosureRequest : PresentationCreateRequest
{
    /// <summary>
    /// Fields to disclose from each credential.
    /// Key: Credential ID, Value: List of field names to disclose.
    /// </summary>
    public Dictionary<string, List<string>> FieldsToDisclose { get; set; } = new();

    /// <summary>
    /// Whether to use zero-knowledge proofs for selective disclosure.
    /// </summary>
    public bool UseZeroKnowledgeProofs { get; set; } = false;

    /// <summary>
    /// ZKP-specific options.
    /// </summary>
    public ZkpOptions ZkpOptions { get; set; } = new();
}

/// <summary>
/// Request for validating a Verifiable Presentation.
/// </summary>
public class PresentationValidationRequest
{
    /// <summary>
    /// The Verifiable Presentation to validate.
    /// </summary>
    public VerifiablePresentation Presentation { get; set; }

    /// <summary>
    /// Optional expected challenge.
    /// </summary>
    public string ExpectedChallenge { get; set; } = string.Empty;

    /// <summary>
    /// Optional expected domain.
    /// </summary>
    public string ExpectedDomain { get; set; } = string.Empty;

    /// <summary>
    /// Whether to check credential revocation status.
    /// </summary>
    public bool CheckRevocationStatus { get; set; } = true;

    /// <summary>
    /// Whether to validate credential schemas.
    /// </summary>
    public bool ValidateSchemas { get; set; } = true;

    /// <summary>
    /// Whether to check issuer trust.
    /// </summary>
    public bool CheckIssuerTrust { get; set; } = true;

    /// <summary>
    /// Trusted issuer DIDs.
    /// </summary>
    public List<string> TrustedIssuers { get; set; } = new();

    /// <summary>
    /// Maximum allowed clock skew in seconds.
    /// </summary>
    public int MaxClockSkewSeconds { get; set; } = 300;

    /// <summary>
    /// Custom validation options.
    /// </summary>
    public Dictionary<string, object> CustomOptions { get; set; } = new();
}

/// <summary>
/// Request for batch validation of Verifiable Presentations.
/// </summary>
public class BatchPresentationValidationRequest
{
    /// <summary>
    /// The Verifiable Presentations to validate.
    /// </summary>
    public List<VerifiablePresentation> Presentations { get; set; } = new();

    /// <summary>
    /// Common validation options for all presentations.
    /// </summary>
    public PresentationValidationRequest CommonOptions { get; set; } = new();

    /// <summary>
    /// Individual validation options per presentation.
    /// Key: Presentation ID, Value: Specific validation options.
    /// </summary>
    public Dictionary<string, PresentationValidationRequest> IndividualOptions { get; set; } = new();
}

/// <summary>
/// Request for creating a ZKP Verifiable Presentation.
/// </summary>
public class ZkpPresentationRequest : PresentationCreateRequest
{
    /// <summary>
    /// ZKP-specific options.
    /// </summary>
    public ZkpOptions ZkpOptions { get; set; } = new();

    /// <summary>
    /// Fields to prove without revealing.
    /// </summary>
    public List<string> HiddenFields { get; set; } = new();

    /// <summary>
    /// Fields to reveal.
    /// </summary>
    public List<string> RevealedFields { get; set; } = new();
}

/// <summary>
/// Request for challenge-response authentication.
/// </summary>
public class ChallengeResponseRequest : PresentationCreateRequest
{
    /// <summary>
    /// The challenge to respond to.
    /// </summary>
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// Optional nonce for additional security.
    /// </summary>
    public string Nonce { get; set; } = string.Empty;

    /// <summary>
    /// Challenge expiration time.
    /// </summary>
    public DateTimeOffset? ChallengeExpiration { get; set; }
}

/// <summary>
/// Request for domain binding.
/// </summary>
public class DomainBindingRequest : PresentationCreateRequest
{
    /// <summary>
    /// The domain to bind to.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Whether to validate domain ownership.
    /// </summary>
    public bool ValidateDomainOwnership { get; set; } = true;
}

/// <summary>
/// Result of Verifiable Presentation validation.
/// </summary>
public class PresentationValidationResult
{
    /// <summary>
    /// Whether the presentation is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings.
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// The holder DID.
    /// </summary>
    public string HolderDid { get; set; } = string.Empty;

    /// <summary>
    /// The validated credentials.
    /// </summary>
    public List<VerifiableCredential> ValidatedCredentials { get; set; } = new();

    /// <summary>
    /// Extracted claims from the presentation.
    /// </summary>
    public Dictionary<string, object> ExtractedClaims { get; set; } = new();

    /// <summary>
    /// Validation metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Result of batch Verifiable Presentation validation.
/// </summary>
public class BatchPresentationValidationResult
{
    /// <summary>
    /// Overall success status.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Individual validation results.
    /// </summary>
    public List<PresentationValidationResult> Results { get; set; } = new();

    /// <summary>
    /// Summary statistics.
    /// </summary>
    public BatchValidationSummary Summary { get; set; } = new();
}

/// <summary>
/// Summary of batch validation results.
/// </summary>
public class BatchValidationSummary
{
    /// <summary>
    /// Total number of presentations validated.
    /// </summary>
    public int TotalPresentations { get; set; }

    /// <summary>
    /// Number of valid presentations.
    /// </summary>
    public int ValidPresentations { get; set; }

    /// <summary>
    /// Number of invalid presentations.
    /// </summary>
    public int InvalidPresentations { get; set; }

    /// <summary>
    /// Total number of errors.
    /// </summary>
    public int TotalErrors { get; set; }

    /// <summary>
    /// Total number of warnings.
    /// </summary>
    public int TotalWarnings { get; set; }

    /// <summary>
    /// Validation duration.
    /// </summary>
    public TimeSpan ValidationDuration { get; set; }
}

/// <summary>
/// ZKP-specific options.
/// </summary>
public class ZkpOptions
{
    /// <summary>
    /// The ZKP proof type to use.
    /// </summary>
    public string ProofType { get; set; } = "BbsBlsSignature2020";

    /// <summary>
    /// ZKP-specific parameters.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// Whether to use selective disclosure.
    /// </summary>
    public bool UseSelectiveDisclosure { get; set; } = true;

    /// <summary>
    /// Fields to hide in the proof.
    /// </summary>
    public List<string> HiddenFields { get; set; } = new();

    /// <summary>
    /// Fields to reveal in the proof.
    /// </summary>
    public List<string> RevealedFields { get; set; } = new();
}




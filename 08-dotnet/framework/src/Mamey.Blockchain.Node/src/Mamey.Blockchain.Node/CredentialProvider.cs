using System.Text.Json;
using Grpc.Core;

namespace Mamey.Blockchain.Node;

/// <summary>
/// Provides credential metadata for gRPC requests
/// </summary>
public class CredentialProvider
{
    /// <summary>
    /// Institution ID (DID format or UUID)
    /// </summary>
    public string? InstitutionId { get; set; }

    /// <summary>
    /// Credential chain for capability verification
    /// </summary>
    public List<Credential>? CredentialChain { get; set; }

    /// <summary>
    /// Generate correlation ID for requests
    /// </summary>
    public Func<string> CorrelationIdGenerator { get; set; } = () => Guid.NewGuid().ToString();

    /// <summary>
    /// Get metadata headers for gRPC requests
    /// </summary>
    public Metadata GetMetadata(string? correlationId = null)
    {
        var metadata = new Metadata();

        // Add correlation ID
        var corrId = correlationId ?? CorrelationIdGenerator();
        metadata.Add("x-correlation-id", corrId);

        // Add institution ID if provided
        if (!string.IsNullOrEmpty(InstitutionId))
        {
            metadata.Add("x-institution-id", InstitutionId);
        }

        // Add credential chain if provided
        if (CredentialChain != null && CredentialChain.Count > 0)
        {
            var credJson = JsonSerializer.Serialize(CredentialChain);
            metadata.Add("x-credential-chain", credJson);
        }

        // Add client info
        metadata.Add("x-client-name", "mamey-dotnet-sdk");
        metadata.Add("x-client-version", "2.0.0");

        return metadata;
    }
}

/// <summary>
/// Credential model for capability verification
/// </summary>
public class Credential
{
    /// <summary>
    /// Credential ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Credential type (e.g., "GovOperatorCredential", "BankCharterCredential")
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Issuer DID
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Subject DID (institution ID)
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Capabilities granted by this credential
    /// </summary>
    public List<string> Capabilities { get; set; } = new();

    /// <summary>
    /// Issued at timestamp
    /// </summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>
    /// Expires at timestamp
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Credential signature
    /// </summary>
    public string Signature { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

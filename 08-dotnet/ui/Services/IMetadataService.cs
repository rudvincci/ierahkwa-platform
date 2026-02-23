using Grpc.Core;

namespace MameyNode.UI.Services;

/// <summary>
/// Service for managing and injecting metadata into gRPC requests
/// </summary>
public interface IMetadataService
{
    /// <summary>
    /// Gets or generates a correlation ID for the current request
    /// </summary>
    string GetCorrelationId();

    /// <summary>
    /// Gets the bearer token (if configured)
    /// </summary>
    string? GetBearerToken();

    /// <summary>
    /// Gets the institution ID (if configured)
    /// </summary>
    string? GetInstitutionId();

    /// <summary>
    /// Gets the credential chain (if configured)
    /// </summary>
    List<Mamey.Blockchain.Node.Credential>? GetCredentialChain();

    /// <summary>
    /// Gets gRPC metadata for requests
    /// </summary>
    Metadata GetMetadata(string? correlationId = null);

    /// <summary>
    /// Sets the institution ID for the current session
    /// </summary>
    void SetInstitutionId(string? institutionId);

    /// <summary>
    /// Sets the credential chain for the current session
    /// </summary>
    void SetCredentialChain(List<Mamey.Blockchain.Node.Credential>? credentialChain);

    /// <summary>
    /// Sets the bearer token for the current session
    /// </summary>
    void SetBearerToken(string? bearerToken);
}

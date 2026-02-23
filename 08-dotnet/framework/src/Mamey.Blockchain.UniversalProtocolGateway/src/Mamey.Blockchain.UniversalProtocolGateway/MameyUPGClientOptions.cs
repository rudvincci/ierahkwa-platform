namespace Mamey.Blockchain.UniversalProtocolGateway;

/// <summary>
/// Configuration options for MameyUPGClient
/// </summary>
public class MameyUPGClientOptions
{
    /// <summary>
    /// The base URL of the MameyNode gRPC service
    /// </summary>
    public string NodeUrl { get; set; } = "http://localhost:50051";

    /// <summary>
    /// Timeout for requests in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Default institution ID for all requests (optional)
    /// </summary>
    public string? DefaultInstitutionId { get; set; }

    /// <summary>
    /// Default credential chain for all requests (optional)
    /// </summary>
    public List<Credential>? DefaultCredentialChain { get; set; }

    /// <summary>
    /// Correlation ID generator function (defaults to Guid.NewGuid)
    /// </summary>
    public Func<string> CorrelationIdGenerator { get; set; } = () => Guid.NewGuid().ToString();
}

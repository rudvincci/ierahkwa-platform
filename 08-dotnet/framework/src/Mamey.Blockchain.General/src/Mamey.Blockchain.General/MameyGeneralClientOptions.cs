namespace Mamey.Blockchain.General;

/// <summary>
/// Configuration options for MameyGeneralClient
/// </summary>
public class MameyGeneralClientOptions
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
    /// Optional TLS/mTLS configuration.
    /// </summary>
    public GrpcTlsOptions? Tls { get; set; }
}

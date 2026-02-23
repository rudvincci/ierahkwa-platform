namespace Mamey.Blockchain.DID;

/// <summary>
/// Configuration options for the DID blockchain client.
/// </summary>
public class DIDClientOptions
{
    /// <summary>
    /// The base URL of the MameyNode gRPC service.
    /// Default: http://localhost:50051
    /// </summary>
    public string NodeUrl { get; set; } = "http://localhost:50051";

    /// <summary>
    /// Timeout for gRPC requests in seconds.
    /// Default: 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to include metadata in DID resolution responses.
    /// Default: true.
    /// </summary>
    public bool IncludeMetadataInResolution { get; set; } = true;

    /// <summary>
    /// The DID method to use when issuing new DIDs.
    /// Default: futurewampum.
    /// </summary>
    public string DefaultMethod { get; set; } = "futurewampum";

    /// <summary>
    /// Maximum number of retry attempts for failed operations.
    /// Default: 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in milliseconds.
    /// Default: 1000ms.
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Optional TLS/mTLS configuration for gRPC (PEM files supported).
    /// Use when the node requires TLS or mutual TLS.
    /// </summary>
    public GrpcTlsOptions? Tls { get; set; }
}

/// <summary>
/// TLS/mTLS settings for gRPC clients.
/// </summary>
public class GrpcTlsOptions
{
    /// <summary>
    /// PEM CA certificate path used to validate the server certificate (optional).
    /// </summary>
    public string? CaCertificatePath { get; set; }

    /// <summary>
    /// PEM client certificate path for mTLS (optional).
    /// </summary>
    public string? ClientCertificatePath { get; set; }

    /// <summary>
    /// PEM client private key path for mTLS (optional; required when ClientCertificatePath is set).
    /// </summary>
    public string? ClientKeyPath { get; set; }

    /// <summary>
    /// DEV-ONLY: Skip server certificate validation (optional).
    /// </summary>
    public bool SkipServerCertificateValidation { get; set; }
}

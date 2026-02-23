namespace Mamey.Blockchain.Wallet;

/// <summary>
/// Configuration options for MameyWalletClient
/// </summary>
public class MameyWalletClientOptions
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

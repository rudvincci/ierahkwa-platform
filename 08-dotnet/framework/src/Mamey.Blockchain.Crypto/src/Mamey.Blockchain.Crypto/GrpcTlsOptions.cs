namespace Mamey.Blockchain.Crypto;

/// <summary>
/// TLS/mTLS options for gRPC channel configuration.
/// </summary>
public class GrpcTlsOptions
{
    public string? CaCertificatePath { get; set; }
    public string? ClientCertificatePath { get; set; }
    public string? ClientKeyPath { get; set; }
    public bool SkipServerCertificateValidation { get; set; }
}


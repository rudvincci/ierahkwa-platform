namespace Mamey.Blockchain.LedgerIntegration;

/// <summary>
/// Configuration options for MameyLedgerClient
/// </summary>
public class MameyLedgerClientOptions
{
    /// <summary>
    /// The base URL of the MameyNode gRPC service
    /// </summary>
    public string NodeUrl { get; set; } = "http://localhost:50051";

    /// <summary>
    /// Timeout for requests in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}

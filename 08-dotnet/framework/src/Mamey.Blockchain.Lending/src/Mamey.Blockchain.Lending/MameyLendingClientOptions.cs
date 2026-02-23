namespace Mamey.Blockchain.Lending;

/// <summary>
/// Configuration options for MameyLendingClient
/// </summary>
public class MameyLendingClientOptions
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

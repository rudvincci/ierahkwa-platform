namespace Mamey.Blockchain.Advanced;

/// <summary>
/// Configuration options for MameyAdvancedClient
/// </summary>
public class MameyAdvancedClientOptions
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

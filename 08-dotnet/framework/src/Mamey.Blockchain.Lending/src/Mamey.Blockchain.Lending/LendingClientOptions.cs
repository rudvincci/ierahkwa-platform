namespace Mamey.Blockchain.Lending;

/// <summary>
/// Options for configuring Lending service client
/// </summary>
public class LendingClientOptions
{
    /// <summary>
    /// Node host address (default: localhost)
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Node gRPC port (default: 50051)
    /// </summary>
    public int Port { get; set; } = 50051;

    /// <summary>
    /// Connection timeout in seconds (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}





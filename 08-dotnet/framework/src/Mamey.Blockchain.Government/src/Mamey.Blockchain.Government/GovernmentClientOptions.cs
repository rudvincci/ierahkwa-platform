namespace Mamey.Blockchain.Government;

/// <summary>
/// Options for configuring Government service client
/// </summary>
public class GovernmentClientOptions
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





namespace Mamey.Blockchain.UniversalProtocolGateway;

public class UniversalProtocolGatewayClientOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 50051;
    public int TimeoutSeconds { get; set; } = 30;
}

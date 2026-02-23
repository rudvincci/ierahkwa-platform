using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Upg;

namespace Mamey.Blockchain.UniversalProtocolGateway;

public class UniversalProtocolGatewayClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly UniversalProtocolGatewayService.UniversalProtocolGatewayServiceClient _client;
    private readonly ILogger<UniversalProtocolGatewayClient>? _logger;
    private readonly UniversalProtocolGatewayClientOptions _options;

    public UniversalProtocolGatewayClient(UniversalProtocolGatewayClientOptions options, ILogger<UniversalProtocolGatewayClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new UniversalProtocolGatewayService.UniversalProtocolGatewayServiceClient(_channel);

        _logger?.LogInformation("UniversalProtocolGateway client initialized for {Address}", address);
    }

    public UniversalProtocolGatewayClient(IOptions<UniversalProtocolGatewayClientOptions> options, ILogger<UniversalProtocolGatewayClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}

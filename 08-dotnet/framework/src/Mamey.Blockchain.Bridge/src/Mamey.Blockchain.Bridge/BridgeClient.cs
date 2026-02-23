using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Bridge;

namespace Mamey.Blockchain.Bridge;

public class BridgeClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly BridgeService.BridgeServiceClient _client;
    private readonly ILogger<BridgeClient>? _logger;
    private readonly BridgeClientOptions _options;

    public BridgeClient(BridgeClientOptions options, ILogger<BridgeClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new BridgeService.BridgeServiceClient(_channel);

        _logger?.LogInformation("Bridge client initialized for {Address}", address);
    }

    public BridgeClient(IOptions<BridgeClientOptions> options, ILogger<BridgeClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
